namespace Degen.Application.MarketData.Dtos;

public record WatchlistDto(Guid Id, string Name, List<WatchlistItemDto> Items);

public record WatchlistItemDto(Guid InstrumentId, string Symbol, string Exchange, int SortOrder);
