using System.Net;
using System.Net.Http.Json;
using Degen.Application.MarketData;

namespace Degen.Infrastructure.MarketData;

internal sealed record CandleDto(
    DateTime Ts,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);

internal sealed record CandlesResponseDto(
    string Symbol,
    string Interval,
    List<CandleDto> Candles
);

internal sealed record QuoteResponseDto(
    string Symbol,
    decimal Price,
    decimal? PreviousClose,
    string? Currency
);

// Talks to the internal marketdata service, which owns the upstream vendor.
public sealed class MarketDataApiPriceProvider(HttpClient http) : IPriceProvider
{
    public async Task<IReadOnlyList<PriceCandle>> GetCandlesAsync(
        string symbol,
        string interval,
        DateOnly since,
        CancellationToken cancellationToken = default
    )
    {
        var uri =
            $"candles?symbol={Uri.EscapeDataString(symbol)}"
            + $"&interval={Uri.EscapeDataString(interval)}&start={since:yyyy-MM-dd}";

        using var response = await http.GetAsync(uri, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CandlesResponseDto>(
            cancellationToken
        );

        return payload is null
            ? []
            : payload
                .Candles.Select(c => new PriceCandle(
                    c.Ts,
                    c.Open,
                    c.High,
                    c.Low,
                    c.Close,
                    c.Volume
                ))
                .ToList();
    }

    public async Task<PriceQuote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken = default
    )
    {
        using var response = await http.GetAsync(
            $"quote?symbol={Uri.EscapeDataString(symbol)}",
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<QuoteResponseDto>(
            cancellationToken
        );

        return payload is null
            ? null
            : new PriceQuote(payload.Price, payload.PreviousClose, payload.Currency);
    }
}
