using Degen.Application.MarketData.Dtos;
using MediatR;

namespace Degen.Application.MarketData.Queries.GetWatchlists;

public record GetWatchlistsQuery : IRequest<IReadOnlyList<WatchlistDto>>;
