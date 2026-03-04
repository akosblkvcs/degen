using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetInstrument;

public record GetInstrumentQuery(Guid Id) : IRequest<InstrumentDto?>;
