using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Commands.CreateWatchlist;

public record CreateWatchlistCommand(string Name) : IRequest<WatchlistDto>;
