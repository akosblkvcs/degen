using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Commands.AddToWatchlist;

public record AddToWatchlistCommand(Guid WatchlistId, Guid InstrumentId) : IRequest<WatchlistDto>;
