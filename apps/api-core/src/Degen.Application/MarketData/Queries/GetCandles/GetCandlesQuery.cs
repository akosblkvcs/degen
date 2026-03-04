using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetCandles;

public record GetCandlesQuery(
    Guid InstrumentId,
    string Interval,
    DateTimeOffset From,
    DateTimeOffset To
) : IRequest<IReadOnlyList<CandleDto>>;
