using Degen.Application.MarketData.Commands.AddToWatchlist;
using Degen.Application.MarketData.Commands.CreateWatchlist;
using Degen.Application.MarketData.Commands.RemoveFromWatchlist;
using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Queries.GetWatchlists;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Degen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchlistsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get all watchlists.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<WatchlistDto>>(200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetWatchlistsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Create a new watchlist.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<WatchlistDto>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWatchlistRequest request,
        CancellationToken ct
    )
    {
        var result = await mediator.Send(new CreateWatchlistCommand(request.Name), ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>
    /// Add an instrument to a watchlist.
    /// </summary>
    [HttpPost("{watchlistId:guid}/instruments/{instrumentId:guid}")]
    [ProducesResponseType<WatchlistDto>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddInstrument(
        Guid watchlistId,
        Guid instrumentId,
        CancellationToken ct
    )
    {
        var result = await mediator.Send(new AddToWatchlistCommand(watchlistId, instrumentId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Remove an instrument from a watchlist.
    /// </summary>
    [HttpDelete("{watchlistId:guid}/instruments/{instrumentId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveInstrument(
        Guid watchlistId,
        Guid instrumentId,
        CancellationToken ct
    )
    {
        await mediator.Send(new RemoveFromWatchlistCommand(watchlistId, instrumentId), ct);
        return NoContent();
    }
}

public record CreateWatchlistRequest(string Name);
