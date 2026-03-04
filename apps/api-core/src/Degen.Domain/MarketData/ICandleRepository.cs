namespace Degen.Domain.MarketData;

public interface ICandleRepository
{
    Task<IReadOnlyList<Candle>> GetCandlesAsync(
        Guid instrumentId,
        string interval,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default
    );

    Task<Candle?> GetLatestCandleAsync(
        Guid instrumentId,
        string interval,
        CancellationToken ct = default
    );

    Task UpsertCandleAsync(Candle candle, CancellationToken ct = default);
    Task UpsertCandlesAsync(IEnumerable<Candle> candles, CancellationToken ct = default);
}
