using Degen.Domain.Common;

namespace Degen.Domain.MarketData;

/// <summary>
/// A tradeable asset: BTC, ETH, USD, AAPL, etc.
/// </summary>
public class Asset : Entity
{
    public string Symbol { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public AssetType Type { get; private set; }
    public int Decimals { get; private set; }

    private Asset() { } // EF Core

    public static Asset Create(string symbol, string name, AssetType type, int decimals)
    {
        return new Asset
        {
            Symbol = symbol.ToUpperInvariant(),
            Name = name,
            Type = type,
            Decimals = decimals,
        };
    }
}

public enum AssetType
{
    Crypto,
    Fiat,
    Stock,
    Etf,
}
