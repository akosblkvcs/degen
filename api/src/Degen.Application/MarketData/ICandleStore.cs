namespace Degen.Application.MarketData;

public interface ICandleStore
{
    Task SaveAsync(
        Guid instrumentId,
        string interval,
        IReadOnlyList<PriceCandle> candles,
        CancellationToken cancellationToken = default
    );
}
