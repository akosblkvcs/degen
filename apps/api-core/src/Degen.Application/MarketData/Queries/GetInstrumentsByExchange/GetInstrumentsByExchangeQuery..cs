using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetInstrumentsByExchange;

public record GetInstrumentsByExchangeQuery(string Exchange)
    : IRequest<IReadOnlyList<InstrumentDto>>;
