using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetInstrumentsByExchange;

public class GetInstrumentsByExchangeHandler(IInstrumentRepository repository)
    : IRequestHandler<GetInstrumentsByExchangeQuery, IReadOnlyList<InstrumentDto>>
{
    public async Task<IReadOnlyList<InstrumentDto>> Handle(
        GetInstrumentsByExchangeQuery request,
        CancellationToken cancellationToken
    )
    {
        var instruments = await repository.GetActiveByExchangeAsync(
            request.Exchange,
            cancellationToken
        );
        return instruments.Select(i => i.ToDto()).ToList();
    }
}
