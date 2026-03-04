using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetWatchlists;

public class GetWatchlistsHandler(IWatchlistRepository repository)
    : IRequestHandler<GetWatchlistsQuery, IReadOnlyList<WatchlistDto>>
{
    public async Task<IReadOnlyList<WatchlistDto>> Handle(
        GetWatchlistsQuery request,
        CancellationToken cancellationToken
    )
    {
        var watchlists = await repository.GetAllAsync(ct: cancellationToken);
        return watchlists.Select(w => w.ToDto()).ToList();
    }
}
