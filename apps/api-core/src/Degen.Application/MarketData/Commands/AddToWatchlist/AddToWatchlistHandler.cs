using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Mappings;
using Degen.Domain.MarketData;
using MediatR;

namespace Degen.Application.MarketData.Commands.AddToWatchlist;

public class AddToWatchlistHandler(
    IWatchlistRepository watchlistRepo,
    IInstrumentRepository instrumentRepo
) : IRequestHandler<AddToWatchlistCommand, WatchlistDto>
{
    public async Task<WatchlistDto> Handle(
        AddToWatchlistCommand request,
        CancellationToken cancellationToken
    )
    {
        var watchlist =
            await watchlistRepo.GetByIdAsync(request.WatchlistId, cancellationToken)
            ?? throw new KeyNotFoundException($"Watchlist {request.WatchlistId} not found");

        var instrument =
            await instrumentRepo.GetByIdAsync(request.InstrumentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Instrument {request.InstrumentId} not found");

        watchlist.AddInstrument(instrument.Id);
        await watchlistRepo.SaveChangesAsync(cancellationToken);

        // Reload to get full navigation properties
        watchlist = await watchlistRepo.GetByIdAsync(request.WatchlistId, cancellationToken);
        return watchlist!.ToDto();
    }
}
