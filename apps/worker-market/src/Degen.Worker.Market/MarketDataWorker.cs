using System.Collections.Concurrent;
using Degen.Contracts.Events;
using Degen.Domain.MarketData;
using Degen.Worker.Market.Exchanges;
using Degen.Worker.Market.Services;

namespace Degen.Worker.Market;

public class MarketDataWorker : BackgroundService
{
    private readonly IExchangeConnection _exchange;
    private readonly SymbolMapper _symbolMapper;
    private readonly CandlePersistenceService _persistence;
    private readonly EventPublisher _publisher;
    private readonly ILogger<MarketDataWorker> _logger;
    private readonly IConfiguration _configuration;
    private CancellationTokenSource? _sessionCts;

    // Track last candle timestamp per symbol + interval to detect closes
    private readonly ConcurrentDictionary<string, DateTimeOffset> _lastCandleTimestamps = new();

    public MarketDataWorker(
        IExchangeConnection exchange,
        SymbolMapper symbolMapper,
        CandlePersistenceService persistence,
        EventPublisher publisher,
        ILogger<MarketDataWorker> logger,
        IConfiguration configuration
    )
    {
        _exchange = exchange;
        _symbolMapper = symbolMapper;
        _persistence = persistence;
        _publisher = publisher;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                LogMessages.WorkerError(_logger, _exchange.ExchangeName, ex);
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                LogMessages.WorkerReconnecting(_logger, _exchange.ExchangeName);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        LogMessages.WorkerStopping(_logger, _exchange.ExchangeName);
    }

    private async Task RunAsync(CancellationToken stoppingToken)
    {
        LogMessages.WorkerStarting(_logger, _exchange.ExchangeName);

        // Session token — cancelled when this session ends
        _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var sessionToken = _sessionCts.Token;

        _lastCandleTimestamps.Clear();

        await _publisher.InitializeAsync(sessionToken);
        await _symbolMapper.LoadMappingsAsync(_exchange.ExchangeName, sessionToken);

        var symbols = _symbolMapper.GetSubscriptionSymbols();

        while (symbols.Count == 0)
        {
            LogMessages.NoInstruments(_logger, _exchange.ExchangeName);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            await _symbolMapper.LoadMappingsAsync(_exchange.ExchangeName, stoppingToken);

            symbols = _symbolMapper.GetSubscriptionSymbols();
        }

        _exchange.CandleReceived += OnCandleReceived;
        _exchange.TickReceived += OnTickReceived;

        try
        {
            await _exchange.ConnectAsync(sessionToken);

            var intervals = _configuration.GetSection("Market:Intervals").Get<string[]>() ?? ["1m"];

            foreach (var symbol in symbols)
            {
                foreach (var interval in intervals)
                {
                    await _exchange.SubscribeOhlcAsync(symbol, interval, sessionToken);
                }
                await _exchange.SubscribeTickerAsync(symbol, sessionToken);
            }

            LogMessages.SubscriptionComplete(
                _logger,
                symbols.Count,
                intervals.Length,
                _exchange.ExchangeName
            );

            // Wait until either cancelled or connection drops
            await Task.WhenAny(_exchange.Disconnected, Task.Delay(Timeout.Infinite, sessionToken));
        }
        finally
        {
            // Cancel session — stops any in-flight callbacks
            await _sessionCts.CancelAsync();

            _exchange.CandleReceived -= OnCandleReceived;
            _exchange.TickReceived -= OnTickReceived;

            await _exchange.DisconnectAsync(CancellationToken.None);

            _sessionCts.Dispose();
            _sessionCts = null;
        }
    }

    private async void OnCandleReceived(object? sender, CandleReceivedEventArgs e)
    {
        var token = _sessionCts?.Token ?? CancellationToken.None;
        if (token.IsCancellationRequested)
            return;

        try
        {
            var instrument = _symbolMapper.Resolve(e.Symbol);
            if (instrument is null)
            {
                LogMessages.UnknownSymbol(_logger, _exchange.ExchangeName, e.Symbol);

                return;
            }

            var candle = new Candle
            {
                InstrumentId = instrument.Id,
                Interval = e.Interval,
                Timestamp = e.Timestamp,
                Open = e.Open,
                High = e.High,
                Low = e.Low,
                Close = e.Close,
                Volume = e.Volume,
            };

            await _persistence.EnqueueAsync(candle, token);

            // Detect candle close: timestamp changed means previous candle closed
            var key = $"{e.Symbol}:{e.Interval}";
            var isClosed = false;

            if (e.Kind == MarketMessageKind.Snapshot)
            {
                // Snapshot — initial state, not a close
                _lastCandleTimestamps[key] = e.Timestamp;
            }
            else
            {
                var previousTimestamp = _lastCandleTimestamps.GetValueOrDefault(key);
                if (previousTimestamp != default && previousTimestamp != e.Timestamp)
                {
                    // Timestamp changed — previous candle is closed
                    isClosed = true;
                }
                _lastCandleTimestamps[key] = e.Timestamp;
            }

            await _publisher.PublishCandleUpdateAsync(
                new CandleUpdateEvent(
                    instrument.Symbol,
                    _exchange.ExchangeName,
                    e.Interval,
                    e.Timestamp,
                    e.Open,
                    e.High,
                    e.Low,
                    e.Close,
                    e.Volume,
                    isClosed
                ),
                token
            );
        }
        catch (OperationCanceledException)
        {
            // Session ended
        }
        catch (Exception ex)
        {
            LogMessages.CandleProcessingError(_logger, _exchange.ExchangeName, e.Symbol, ex);
        }
    }

    private async void OnTickReceived(object? sender, TickReceivedEventArgs e)
    {
        var cts = _sessionCts;

        if (cts is null || cts.IsCancellationRequested)
            return;

        var token = cts.Token;

        try
        {
            var instrument = _symbolMapper.Resolve(e.Symbol);
            if (instrument is null)
                return;

            await _publisher.PublishTickAsync(
                instrument.Symbol,
                _exchange.ExchangeName,
                e.Price,
                e.Volume24h,
                e.Timestamp,
                token
            );
        }
        catch (OperationCanceledException)
        {
            // Session ended — expected during reconnect
        }
        catch (Exception ex)
        {
            LogMessages.TickProcessingError(_logger, _exchange.ExchangeName, e.Symbol, ex);
        }
    }
}
