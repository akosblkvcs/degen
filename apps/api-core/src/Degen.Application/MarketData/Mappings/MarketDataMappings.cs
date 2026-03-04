using Degen.Application.MarketData.Dtos;
using Degen.Domain.MarketData;

namespace Degen.Application.MarketData.Mappings;

public static class MarketDataMappings
{
    public static AssetDto ToDto(this Asset asset) =>
        new(asset.Id, asset.Symbol, asset.Name, asset.Type.ToString(), asset.Decimals);

    public static InstrumentDto ToDto(this Instrument instrument) =>
        new(
            instrument.Id,
            instrument.Symbol,
            instrument.ExchangeSymbol,
            instrument.Exchange,
            instrument.BaseAsset.ToDto(),
            instrument.QuoteAsset.ToDto(),
            instrument.PriceDecimals,
            instrument.QuantityDecimals,
            instrument.MinOrderSize,
            instrument.IsActive
        );

    public static CandleDto ToDto(this Candle candle) =>
        new(candle.Timestamp, candle.Open, candle.High, candle.Low, candle.Close, candle.Volume);

    public static WatchlistDto ToDto(this Watchlist watchlist) =>
        new(
            watchlist.Id,
            watchlist.Name,
            watchlist
                .Items.Select(i => new WatchlistItemDto(
                    i.InstrumentId,
                    i.Instrument?.Symbol ?? "",
                    i.Instrument?.Exchange ?? "",
                    i.SortOrder
                ))
                .ToList()
        );
}
