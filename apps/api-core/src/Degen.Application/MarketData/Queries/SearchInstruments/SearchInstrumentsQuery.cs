using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Queries.SearchInstruments;

public record SearchInstrumentsQuery(string Query) : IRequest<IReadOnlyList<InstrumentDto>>;
