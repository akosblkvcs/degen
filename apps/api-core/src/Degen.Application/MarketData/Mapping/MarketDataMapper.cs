using Degen.Application.MarketData.Dtos;
using Degen.Domain.MarketData;
using Riok.Mapperly.Abstractions;

namespace Degen.Application.MarketData.Mappings;

[Mapper]
public static partial class MarketDataMapper
{
    [MapperIgnoreSource(nameof(Asset.DomainEvents))]
    public static partial AssetDto ToDto(this Asset asset);

    [MapperIgnoreSource(nameof(Instrument.DomainEvents))]
    [MapperIgnoreSource(nameof(Instrument.BaseAssetId))]
    [MapperIgnoreSource(nameof(Instrument.QuoteAssetId))]
    public static partial InstrumentDto ToDto(this Instrument instrument);

    [MapperIgnoreSource(nameof(Candle.InstrumentId))]
    [MapperIgnoreSource(nameof(Candle.Interval))]
    public static partial CandleDto ToDto(this Candle candle);

    [MapperIgnoreSource(nameof(Watchlist.DomainEvents))]
    [MapperIgnoreSource(nameof(Watchlist.TenantId))]
    public static partial WatchlistDto ToDto(this Watchlist watchlist);

    [MapperIgnoreSource(nameof(WatchlistItem.Id))]
    [MapperIgnoreSource(nameof(WatchlistItem.WatchlistId))]
    [MapProperty(nameof(WatchlistItem.Instrument.Symbol), nameof(WatchlistItemDto.Symbol))]
    [MapProperty(nameof(WatchlistItem.Instrument.Exchange), nameof(WatchlistItemDto.Exchange))]
    private static partial WatchlistItemDto ToDto(this WatchlistItem item);
}
