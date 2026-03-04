using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Queries.SearchInstruments;

public class SearchInstrumentsHandler(IInstrumentRepository repository)
    : IRequestHandler<SearchInstrumentsQuery, IReadOnlyList<InstrumentDto>>
{
    public async Task<IReadOnlyList<InstrumentDto>> Handle(
        SearchInstrumentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var instruments = await repository.SearchAsync(request.Query, cancellationToken);
        return instruments.Select(i => i.ToDto()).ToList();
    }
}
