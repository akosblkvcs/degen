using Degen.Domain.Instruments;
using Degen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Degen.Api.Endpoints;

public record CreateInstrumentRequest(string? Symbol, string? Name, string? AssetType);

public static class InstrumentEndpoints
{
    private const string DefaultUserId = "default";

    public static IEndpointRouteBuilder MapInstrumentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/instruments");

        group.MapGet("/", async (AppDbContext db, CancellationToken cancellationToken) =>
            await db.Instruments
                .AsNoTracking()
                .Where(i => i.UserId == DefaultUserId)
                .OrderBy(i => i.Symbol)
                .ToListAsync(cancellationToken));

        group.MapPost("/", async (
            CreateInstrumentRequest request,
            AppDbContext db,
            CancellationToken cancellationToken) =>
        {
            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(request.Symbol))
                errors["symbol"] = ["Symbol is required."];
            if (string.IsNullOrWhiteSpace(request.Name))
                errors["name"] = ["Name is required."];
            if (string.IsNullOrWhiteSpace(request.AssetType))
                errors["assetType"] = ["AssetType is required."];
            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            var instrument = new Instrument
            {
                Id = Guid.CreateVersion7(),
                Symbol = request.Symbol!.Trim().ToUpperInvariant(),
                Name = request.Name!.Trim(),
                AssetType = request.AssetType!.Trim().ToLowerInvariant(),
                UserId = DefaultUserId,
                CreatedAt = DateTime.UtcNow,
            };

            db.Instruments.Add(instrument);
            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return Results.Conflict(
                    new { message = $"Instrument '{instrument.Symbol}' is already on the list." });
            }

            return Results.Created($"/api/instruments/{instrument.Id}", instrument);
        });

        return routes;
    }
}
