using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetInstrument;

public class GetInstrumentHandler(IInstrumentRepository repository)
    : IRequestHandler<GetInstrumentQuery, InstrumentDto?>
{
    public async Task<InstrumentDto?> Handle(
        GetInstrumentQuery request,
        CancellationToken cancellationToken
    )
    {
        var instrument = await repository.GetByIdAsync(request.Id, cancellationToken);
        return instrument?.ToDto();
    }
}
