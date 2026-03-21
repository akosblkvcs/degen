using System.Threading.Channels;
using Degen.Domain.MarketData;
using Degen.Infrastructure.Persistence;

namespace Degen.Worker.Market.Services;

/// <summary>
/// Batches candle writes to avoid per-message DB round trips.
/// </summary>
public class CandlePersistenceService : BackgroundService
{
    private readonly Channel<Candle> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CandlePersistenceService> _logger;

    public CandlePersistenceService(
        IServiceScopeFactory scopeFactory,
        ILogger<CandlePersistenceService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _channel = Channel.CreateBounded<Candle>(
            new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
            }
        );
    }

    public ValueTask EnqueueAsync(Candle candle, CancellationToken ct)
    {
        return _channel.Writer.WriteAsync(candle, ct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogMessages.PersistenceServiceStarted(_logger);

        var batch = new List<Candle>(100);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (batch.Count == 0)
            {
                batch.Add(await _channel.Reader.ReadAsync(stoppingToken));
            }

            // Drain available items without blocking
            while (batch.Count < 100 && _channel.Reader.TryRead(out var extra))
            {
                batch.Add(extra);
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var candleRepo = scope.ServiceProvider.GetRequiredService<ICandleRepository>();

                await candleRepo.UpsertCandlesAsync(batch, stoppingToken);

                LogMessages.CandlesPersisted(_logger, batch.Count);

                // Only clear after successful persistence
                batch.Clear();
            }
            catch (Exception ex)
            {
                LogMessages.PersistenceFailed(_logger, batch.Count, ex);

                // Batch retained — will retry on next channel read
                // Brief delay to avoid tight retry loop on persistent DB failure
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        // Drain and persist remaining items
        var finalBatch = new List<Candle>();
        while (_channel.Reader.TryRead(out var candle))
        {
            finalBatch.Add(candle);
        }

        if (finalBatch.Count > 0)
        {
            using var scope = _scopeFactory.CreateScope();
            var candleRepo = scope.ServiceProvider.GetRequiredService<ICandleRepository>();
            await candleRepo.UpsertCandlesAsync(finalBatch, cancellationToken);
            LogMessages.CandlesPersisted(_logger, finalBatch.Count);
        }
    }
}
