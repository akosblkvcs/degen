using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetCandles;

public class GetCandlesHandler(ICandleRepository repository)
    : IRequestHandler<GetCandlesQuery, IReadOnlyList<CandleDto>>
{
    public async Task<IReadOnlyList<CandleDto>> Handle(
        GetCandlesQuery request,
        CancellationToken cancellationToken
    )
    {
        var candles = await repository.GetCandlesAsync(
            request.InstrumentId,
            request.Interval,
            request.From,
            request.To,
            cancellationToken
        );

        return candles.Select(c => c.ToDto()).ToList();
    }
}
