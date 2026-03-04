using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Commands.RemoveFromWatchlist;

public class RemoveFromWatchlistHandler(IWatchlistRepository repository)
    : IRequestHandler<RemoveFromWatchlistCommand>
{
    public async Task Handle(
        RemoveFromWatchlistCommand request,
        CancellationToken cancellationToken
    )
    {
        var watchlist =
            await repository.GetByIdAsync(request.WatchlistId, cancellationToken)
            ?? throw new KeyNotFoundException($"Watchlist {request.WatchlistId} not found");

        watchlist.RemoveInstrument(request.InstrumentId);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
