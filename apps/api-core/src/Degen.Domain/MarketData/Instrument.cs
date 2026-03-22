using Degen.Domain.Common;

namespace Degen.Domain.MarketData;

/// <summary>
/// A tradeable pair on a specific exchange: BTC/USD on Kraken, AAPL on Alpaca, etc.
/// </summary>
public class Instrument : Entity
{
    public string Symbol { get; private set; } = default!;
    public string ExchangeSymbol { get; private set; } = default!;
    public string Exchange { get; private set; } = default!;

    public Guid BaseAssetId { get; private set; }
    public Asset BaseAsset { get; private set; } = default!;

    public Guid QuoteAssetId { get; private set; }
    public Asset QuoteAsset { get; private set; } = default!;

    public int PriceDecimals { get; private set; }
    public int QuantityDecimals { get; private set; }
    public decimal? MinOrderSize { get; private set; }
    public bool IsActive { get; private set; }

    private Instrument() { }

    public static Instrument Create(
        string symbol,
        string exchangeSymbol,
        string exchange,
        Guid baseAssetId,
        Guid quoteAssetId,
        int priceDecimals,
        int quantityDecimals,
        decimal? minOrderSize
    )
    {
        return new Instrument
        {
            Symbol = symbol,
            ExchangeSymbol = exchangeSymbol,
            Exchange = exchange,
            BaseAssetId = baseAssetId,
            QuoteAssetId = quoteAssetId,
            PriceDecimals = priceDecimals,
            QuantityDecimals = quantityDecimals,
            MinOrderSize = minOrderSize,
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
