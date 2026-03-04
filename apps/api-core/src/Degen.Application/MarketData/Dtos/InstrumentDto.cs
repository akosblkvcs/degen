namespace Degen.Application.MarketData.Dtos;

public record InstrumentDto(
    Guid Id,
    string Symbol,
    string ExchangeSymbol,
    string Exchange,
    AssetDto BaseAsset,
    AssetDto QuoteAsset,
    int PriceDecimals,
    int QuantityDecimals,
    decimal? MinOrderSize,
    bool IsActive
);
