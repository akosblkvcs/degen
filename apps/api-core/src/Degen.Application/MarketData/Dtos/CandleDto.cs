namespace Degen.Application.MarketData.Dtos;

public record CandleDto(
    DateTimeOffset Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
);
