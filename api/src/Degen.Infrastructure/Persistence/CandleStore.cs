using Degen.Application.MarketData;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Degen.Infrastructure.Persistence;

public sealed class CandleStore(AppDbContext db) : ICandleStore
{
    private const string Sql = """
        INSERT INTO candles (instrument_id, "interval", ts, open, high, low, close, volume)
        SELECT @instrument_id, @interval, t.ts, t.open, t.high, t.low, t.close, t.volume
        FROM unnest(@ts, @open, @high, @low, @close, @volume)
            AS t(ts, open, high, low, close, volume)
        ON CONFLICT (instrument_id, "interval", ts) DO UPDATE SET
            open = EXCLUDED.open,
            high = EXCLUDED.high,
            low = EXCLUDED.low,
            close = EXCLUDED.close,
            volume = EXCLUDED.volume
        """;

    public async Task SaveAsync(
        Guid instrumentId,
        string interval,
        IReadOnlyList<PriceCandle> candles,
        CancellationToken cancellationToken = default
    )
    {
        if (candles.Count == 0)
        {
            return;
        }

        object[] parameters =
        [
            new NpgsqlParameter("instrument_id", instrumentId),
            new NpgsqlParameter("interval", interval),
            new NpgsqlParameter("ts", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz)
            {
                Value = candles.Select(c => c.Ts).ToArray(),
            },
            new NpgsqlParameter("open", NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = candles.Select(c => c.Open).ToArray(),
            },
            new NpgsqlParameter("high", NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = candles.Select(c => c.High).ToArray(),
            },
            new NpgsqlParameter("low", NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = candles.Select(c => c.Low).ToArray(),
            },
            new NpgsqlParameter("close", NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = candles.Select(c => c.Close).ToArray(),
            },
            new NpgsqlParameter("volume", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = candles.Select(c => c.Volume).ToArray(),
            },
        ];

        await db.Database.ExecuteSqlRawAsync(Sql, parameters, cancellationToken);
    }
}
