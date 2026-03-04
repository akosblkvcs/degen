using MediatR;

namespace Degen.Application.MarketData.Commands.RemoveFromWatchlist;

public record RemoveFromWatchlistCommand(Guid WatchlistId, Guid InstrumentId) : IRequest;
