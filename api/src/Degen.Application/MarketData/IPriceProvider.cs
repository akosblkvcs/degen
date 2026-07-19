namespace Degen.Application.MarketData;

public record PriceCandle(
    DateTime Ts,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);

public record PriceQuote(decimal Price, decimal? PreviousClose, string? Currency);

public interface IPriceProvider
{
    Task<IReadOnlyList<PriceCandle>> GetCandlesAsync(
        string symbol,
        string interval,
        DateOnly since,
        CancellationToken cancellationToken = default
    );

    Task<PriceQuote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );
}
