using Degen.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Degen.Application.Instruments;

public record GetInstrumentResponse(
    Guid Id,
    string Symbol,
    string Name,
    string AssetType,
    DateTime CreatedAt
);

public class GetInstrumentHandler(IAppDbContext db)
{
    public Task<GetInstrumentResponse?> HandleAsync(
        Guid instrumentId,
        CancellationToken cancellationToken
    ) =>
        db
            .Instruments.AsNoTracking()
            .Where(i => i.Id == instrumentId)
            .Select(i => new GetInstrumentResponse(
                i.Id,
                i.Symbol,
                i.Name,
                i.AssetType,
                i.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
}
