using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Queries.GetInstrument;
using Degen.Application.MarketData.Queries.GetInstrumentsByExchange;
using Degen.Application.MarketData.Queries.SearchInstruments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Degen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstrumentsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Search instruments by symbol or asset name.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType<IReadOnlyList<InstrumentDto>>(200)]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query parameter 'q' is required.");

        var results = await mediator.Send(new SearchInstrumentsQuery(q), ct);
        return Ok(results);
    }

    /// <summary>
    /// Get a single instrument by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<InstrumentDto>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetInstrumentQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// List all active instruments for an exchange.
    /// </summary>
    [HttpGet("exchange/{exchange}")]
    [ProducesResponseType<IReadOnlyList<InstrumentDto>>(200)]
    public async Task<IActionResult> GetByExchange(string exchange, CancellationToken ct)
    {
        var results = await mediator.Send(new GetInstrumentsByExchangeQuery(exchange), ct);
        return Ok(results);
    }
}
