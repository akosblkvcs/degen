using System.Collections.Concurrent;
using System.Text.Json;

namespace Degen.Worker.Market.Exchanges.Kraken;

public class KrakenConnection : IExchangeConnection, IDisposable
{
    private readonly KrakenWebSocketClient _client;
    private readonly ILogger<KrakenConnection> _logger;
    private readonly string _wsUrl;
    private readonly ConcurrentDictionary<string, PendingSubscription> _pendingSubscriptions =
        new();

    public string ExchangeName => "Kraken";
    public Task Disconnected => _client.Disconnected;

    public event EventHandler<CandleReceivedEventArgs>? CandleReceived;
    public event EventHandler<TickReceivedEventArgs>? TickReceived;

    public KrakenConnection(
        KrakenWebSocketClient client,
        ILogger<KrakenConnection> logger,
        IConfiguration configuration
    )
    {
        _client = client;
        _logger = logger;
        _wsUrl = configuration["Kraken:WsUrl"] ?? "wss://ws.kraken.com/v2";

        _client.MessageReceived += OnMessageReceived;
    }

    public async Task ConnectAsync(CancellationToken ct)
    {
        await _client.ConnectAsync(_wsUrl, ct);
    }

    public async Task DisconnectAsync(CancellationToken ct)
    {
        await _client.DisconnectAsync(ct);
    }

    public async Task SubscribeOhlcAsync(string symbol, string interval, CancellationToken ct)
    {
        var krakenInterval = MapInterval(interval);
        var key = $"ohlc:{symbol}:{krakenInterval}";

        var pending = new PendingSubscription { Channel = "ohlc", Symbol = symbol };
        _pendingSubscriptions[key] = pending;

        var message = new
        {
            Method = "subscribe",
            Params = new
            {
                Channel = "ohlc",
                Symbol = new[] { symbol },
                Interval = krakenInterval,
            },
        };

        await _client.SendAsync(message, ct);

        // Wait for acknowledgement with timeout
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

        try
        {
            var success = await pending.Completion.Task.WaitAsync(timeoutCts.Token);
            if (success)
            {
                LogMessages.SubscribedOhlc(_logger, symbol, interval, ExchangeName);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Subscription rejected for ohlc:{symbol} ({interval}) on {ExchangeName}"
                );
            }
        }
        catch (OperationCanceledException)
        {
            // Distinguish caller cancellation from timeout
            ct.ThrowIfCancellationRequested();

            throw new TimeoutException(
                $"Subscription timeout for ohlc:{symbol} ({interval}) on {ExchangeName}"
            );
        }
        finally
        {
            _pendingSubscriptions.TryRemove(key, out _);
        }
    }

    public async Task SubscribeTickerAsync(string symbol, CancellationToken ct)
    {
        var key = $"ticker:{symbol}";

        var pending = new PendingSubscription { Channel = "ticker", Symbol = symbol };
        _pendingSubscriptions[key] = pending;

        var message = new
        {
            Method = "subscribe",
            Params = new { Channel = "ticker", Symbol = new[] { symbol } },
        };

        await _client.SendAsync(message, ct);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

        try
        {
            var success = await pending.Completion.Task.WaitAsync(timeoutCts.Token);
            if (success)
            {
                LogMessages.SubscribedTicker(_logger, symbol, ExchangeName);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Subscription rejected for ticker:{symbol} on {ExchangeName}"
                );
            }
        }
        catch (OperationCanceledException)
        {
            // Distinguish caller cancellation from timeout
            ct.ThrowIfCancellationRequested();

            throw new TimeoutException(
                $"Subscription timeout for ticker:{symbol} on {ExchangeName}"
            );
        }
        finally
        {
            _pendingSubscriptions.TryRemove(key, out _);
        }
    }

    private void OnMessageReceived(object? sender, JsonElement root)
    {
        try
        {
            // Handle subscription acknowledgements
            if (root.TryGetProperty("method", out var methodProp))
            {
                var method = methodProp.GetString();
                if (method == "subscribe")
                {
                    HandleSubscribeAck(root);
                }
                return;
            }

            // Handle errors
            if (root.TryGetProperty("error", out var errorProp))
            {
                LogMessages.ExchangeError(
                    _logger,
                    ExchangeName,
                    errorProp.GetString() ?? "unknown error"
                );
                return;
            }

            if (!root.TryGetProperty("channel", out var channelProp))
                return;

            var channel = channelProp.GetString();

            var kind = MarketMessageKind.Update;
            if (root.TryGetProperty("type", out var typeProp))
            {
                kind =
                    typeProp.GetString() == "snapshot"
                        ? MarketMessageKind.Snapshot
                        : MarketMessageKind.Update;
            }

            switch (channel)
            {
                case "ohlc":
                    ParseOhlcMessage(root, kind);
                    break;
                case "ticker":
                    ParseTickerMessage(root, kind);
                    break;
            }
        }
        catch (Exception ex)
        {
            LogMessages.ParseFailed(_logger, ExchangeName, ex);
        }
    }

    private void HandleSubscribeAck(JsonElement root)
    {
        var success =
            root.TryGetProperty("success", out var successProp) && successProp.GetBoolean();

        if (!root.TryGetProperty("result", out var result))
            return;

        var channel = result.TryGetProperty("channel", out var ch) ? ch.GetString() : null;
        var symbol = result.TryGetProperty("symbol", out var sym) ? sym.GetString() : null;

        if (channel is null || symbol is null)
            return;

        // Build the key to match the pending subscription
        string key;
        if (channel == "ohlc" && result.TryGetProperty("interval", out var intervalProp))
        {
            key = $"ohlc:{symbol}:{intervalProp.GetInt32()}";
        }
        else
        {
            key = $"{channel}:{symbol}";
        }

        if (_pendingSubscriptions.TryGetValue(key, out var pending))
        {
            pending.Completion.TrySetResult(success);
        }

        if (!success)
        {
            var error = root.TryGetProperty("error", out var errProp)
                ? errProp.GetString()
                : "unknown";

            LogMessages.ExchangeSubscribeError(
                _logger,
                ExchangeName,
                channel,
                symbol,
                error ?? "unknown"
            );
        }
    }

    private void ParseOhlcMessage(JsonElement root, MarketMessageKind kind)
    {
        if (!root.TryGetProperty("data", out var dataArray))
            return;

        foreach (var item in dataArray.EnumerateArray())
        {
            var symbol = item.GetProperty("symbol").GetString()!;
            var timestamp = item.GetProperty("interval_begin").GetDateTimeOffset();
            var open = item.GetProperty("open").GetDecimal();
            var high = item.GetProperty("high").GetDecimal();
            var low = item.GetProperty("low").GetDecimal();
            var close = item.GetProperty("close").GetDecimal();
            var volume = item.GetProperty("volume").GetDecimal();
            var intervalMinutes = item.GetProperty("interval").GetInt32();

            CandleReceived?.Invoke(
                this,
                new CandleReceivedEventArgs(
                    symbol,
                    MapIntervalBack(intervalMinutes),
                    timestamp,
                    open,
                    high,
                    low,
                    close,
                    volume,
                    kind
                )
            );
        }
    }

    private void ParseTickerMessage(JsonElement root, MarketMessageKind kind)
    {
        if (!root.TryGetProperty("data", out var dataArray))
            return;

        foreach (var item in dataArray.EnumerateArray())
        {
            var symbol = item.GetProperty("symbol").GetString()!;
            var last = item.GetProperty("last").GetDecimal();
            var volume = item.GetProperty("volume").GetDecimal();

            // Use exchange-provided timestamp instead of DateTimeOffset.UtcNow
            var timestamp = item.TryGetProperty("timestamp", out var tsProp)
                ? tsProp.GetDateTimeOffset()
                : DateTimeOffset.UtcNow;

            TickReceived?.Invoke(
                this,
                new TickReceivedEventArgs(symbol, last, volume, timestamp, kind)
            );
        }
    }

    private static int MapInterval(string interval) =>
        interval switch
        {
            "1m" => 1,
            "5m" => 5,
            "15m" => 15,
            "1h" => 60,
            "4h" => 240,
            "1d" => 1440,
            _ => throw new ArgumentException($"Unsupported interval: {interval}"),
        };

    private static string MapIntervalBack(int minutes) =>
        minutes switch
        {
            1 => "1m",
            5 => "5m",
            15 => "15m",
            60 => "1h",
            240 => "4h",
            1440 => "1d",
            _ => $"{minutes}m",
        };

    public void Dispose()
    {
        _client.MessageReceived -= OnMessageReceived;
        (_client as IDisposable)?.Dispose();

        GC.SuppressFinalize(this);
    }
}
