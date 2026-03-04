using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Commands.CreateWatchlist;

public class CreateWatchlistHandler(IWatchlistRepository repository)
    : IRequestHandler<CreateWatchlistCommand, WatchlistDto>
{
    public async Task<WatchlistDto> Handle(
        CreateWatchlistCommand request,
        CancellationToken cancellationToken
    )
    {
        var watchlist = Watchlist.Create(request.Name);

        await repository.AddAsync(watchlist, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return watchlist.ToDto();
    }
}
