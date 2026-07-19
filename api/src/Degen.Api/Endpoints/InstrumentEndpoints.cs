using Degen.Api.Filters;
using Degen.Application.Instruments;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Degen.Api.Endpoints;

public static class InstrumentEndpoints
{
    // Route names double as OpenAPI operationIds, so they are part of the public
    // contract that generated clients derive method names from.
    private static class RouteNames
    {
        public const string List = "ListInstruments";
        public const string Get = "GetInstrument";
        public const string Quote = "GetQuote";
        public const string Add = "AddInstrument";
    }

    public static IEndpointRouteBuilder MapInstrumentEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        var group = routes.MapGroup("/api/instruments");

        group.MapGet("/", GetAll).WithName(RouteNames.List);
        group.MapGet("/{id:guid}", GetById).WithName(RouteNames.Get);
        group.MapGet("/{id:guid}/quote", GetQuote).WithName(RouteNames.Quote);
        group
            .MapPost("/", Create)
            .WithName(RouteNames.Add)
            .WithValidation<AddInstrumentCommand>();

        return routes;
    }

    private static async Task<Ok<IReadOnlyList<ListInstrumentsItem>>> GetAll(
        ListInstrumentsHandler handler,
        CancellationToken cancellationToken
    ) => TypedResults.Ok(await handler.HandleAsync(cancellationToken));

    private static async Task<
        Results<Ok<GetInstrumentResponse>, ProblemHttpResult>
    > GetById(
        Guid id,
        GetInstrumentHandler handler,
        CancellationToken cancellationToken
    ) =>
        await handler.HandleAsync(id, cancellationToken) is { } instrument
            ? TypedResults.Ok(instrument)
            : NotFound("Instrument not found.");

    private static async Task<Results<Ok<GetQuoteResponse>, ProblemHttpResult>> GetQuote(
        Guid id,
        GetQuoteHandler handler,
        CancellationToken cancellationToken
    ) =>
        await handler.HandleAsync(id, cancellationToken) switch
        {
            GetQuoteResult.Success(var quote) => TypedResults.Ok(quote),
            GetQuoteResult.QuoteUnavailable(var symbol) => NotFound(
                $"No quote available for '{symbol}'."
            ),
            _ => NotFound("Instrument not found."),
        };

    private static async Task<
        Results<
            CreatedAtRoute<AddInstrumentResponse>,
            ProblemHttpResult,
            ValidationProblem
        >
    > Create(
        AddInstrumentCommand command,
        AddInstrumentHandler handler,
        CancellationToken cancellationToken
    ) =>
        await handler.HandleAsync(command, cancellationToken) switch
        {
            AddInstrumentResult.Success(var instrument) => Created(instrument),
            AddInstrumentResult.Duplicate(var symbol) => Conflict(
                $"Instrument '{symbol}' is already on the list."
            ),
            AddInstrumentResult.UnknownSymbol(var symbol) => InvalidSymbol(
                $"Symbol '{symbol}' is unknown to the market data provider."
            ),
            _ => throw new InvalidOperationException("Unhandled result."),
        };

    private static CreatedAtRoute<AddInstrumentResponse> Created(
        AddInstrumentResponse instrument
    ) =>
        TypedResults.CreatedAtRoute(
            instrument,
            RouteNames.Get,
            new { id = instrument.Id }
        );

    private static ProblemHttpResult NotFound(string detail) =>
        TypedResults.Problem(detail, statusCode: StatusCodes.Status404NotFound);

    private static ProblemHttpResult Conflict(string detail) =>
        TypedResults.Problem(detail, statusCode: StatusCodes.Status409Conflict);

    private static ValidationProblem InvalidSymbol(string message) =>
        TypedResults.ValidationProblem(
            new Dictionary<string, string[]> { ["symbol"] = [message] }
        );
}
