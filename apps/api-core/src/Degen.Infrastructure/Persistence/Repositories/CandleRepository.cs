using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;

namespace Degen.Infrastructure.Persistence.Repositories;

public class CandleRepository(AppDbContext db) : ICandleRepository
{
    public async Task<IReadOnlyList<Candle>> GetCandlesAsync(
        Guid instrumentId,
        string interval,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default
    ) =>
        await db
            .Candles.Where(c =>
                c.InstrumentId == instrumentId
                && c.Interval == interval
                && c.Timestamp >= from
                && c.Timestamp <= to
            )
            .OrderBy(c => c.Timestamp)
            .ToListAsync(ct);

    public async Task<Candle?> GetLatestCandleAsync(
        Guid instrumentId,
        string interval,
        CancellationToken ct = default
    ) =>
        await db
            .Candles.Where(c => c.InstrumentId == instrumentId && c.Interval == interval)
            .OrderByDescending(c => c.Timestamp)
            .FirstOrDefaultAsync(ct);

    public async Task UpsertCandleAsync(Candle candle, CancellationToken ct = default)
    {
        var existing = await db.Candles.FindAsync(
            [candle.InstrumentId, candle.Interval, candle.Timestamp],
            ct
        );

        if (existing is null)
        {
            await db.Candles.AddAsync(candle, ct);
        }
        else
        {
            existing.Open = candle.Open;
            existing.High = candle.High;
            existing.Low = candle.Low;
            existing.Close = candle.Close;
            existing.Volume = candle.Volume;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task UpsertCandlesAsync(
        IEnumerable<Candle> candles,
        CancellationToken ct = default
    )
    {
        foreach (var candle in candles)
        {
            var existing = await db.Candles.FindAsync(
                [candle.InstrumentId, candle.Interval, candle.Timestamp],
                ct
            );

            if (existing is null)
            {
                await db.Candles.AddAsync(candle, ct);
            }
            else
            {
                existing.Open = candle.Open;
                existing.High = candle.High;
                existing.Low = candle.Low;
                existing.Close = candle.Close;
                existing.Volume = candle.Volume;
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
