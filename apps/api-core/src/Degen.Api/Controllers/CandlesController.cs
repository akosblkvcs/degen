using Degen.Application.MarketData.Dtos;
using Degen.Application.MarketData.Queries.GetCandles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Degen.Api.Controllers;

[ApiController]
[Route("api/instruments/{instrumentId:guid}/candles")]
public class CandlesController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Get OHLCV candles for an instrument.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CandleDto>>(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetCandles(
        Guid instrumentId,
        [FromQuery] string interval,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct
    )
    {
        var result = await mediator.Send(new GetCandlesQuery(instrumentId, interval, from, to), ct);

        return Ok(result);
    }
}
