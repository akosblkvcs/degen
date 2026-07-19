using Degen.Application.MarketData;
using Degen.Domain.MarketData;
using Degen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Degen.Infrastructure.MarketData;

public sealed class DailyCandleRefreshJob(
    IServiceScopeFactory scopeFactory,
    ILogger<DailyCandleRefreshJob> logger
) : BackgroundService
{
    // A rolling month per run: covers downtime gaps, stays tiny, and the idempotent
    // save makes re-fetching overlapping days harmless.
    private const int RefreshWindowDays = 31;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Catch-up pass on startup, then aligned daily runs.
        await RefreshAllAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = DelayUntilNextRun(DateTime.UtcNow, 22);

            logger.LogInformation("Next candle refresh in {Delay:hh\\:mm\\:ss}", delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await RefreshAllAsync(stoppingToken);
        }
    }

    internal static TimeSpan DelayUntilNextRun(DateTime utcNow, int hourUtc)
    {
        var next = utcNow.Date.AddHours(hourUtc);

        if (next <= utcNow)
        {
            next = next.AddDays(1);
        }

        return next - utcNow;
    }

    private async Task RefreshAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var priceProvider =
                scope.ServiceProvider.GetRequiredService<IPriceProvider>();
            var candleStore = scope.ServiceProvider.GetRequiredService<ICandleStore>();

            var instruments = await db
                .Instruments.AsNoTracking()
                .ToListAsync(cancellationToken);

            var stored = 0;
            var failed = 0;
            foreach (var instrument in instruments)
            {
                try
                {
                    var candles = await priceProvider.GetCandlesAsync(
                        instrument.Symbol,
                        Interval.Daily,
                        DateOnly.FromDateTime(
                            DateTime.UtcNow.AddDays(-RefreshWindowDays)
                        ),
                        cancellationToken
                    );

                    await candleStore.SaveAsync(
                        instrument.Id,
                        Interval.Daily,
                        candles,
                        cancellationToken
                    );

                    stored += candles.Count;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    failed++;

                    logger.LogWarning(
                        ex,
                        "Candle refresh failed for {Symbol}",
                        instrument.Symbol
                    );
                }
            }

            logger.LogInformation(
                "Candle refresh stored {Stored} candles across {Count} instruments ({Failed} failed)",
                stored,
                instruments.Count,
                failed
            );
        }
        catch (OperationCanceledException)
        {
            // Shutdown requested mid-run: exit quietly.
        }
        catch (Exception ex)
        {
            // Never let an unhandled exception escape: by default that stops the whole host.
            logger.LogError(ex, "Candle refresh run failed");
        }
    }
}
