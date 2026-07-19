using Degen.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Degen.Application.Instruments;

public record ListInstrumentsItem(
    Guid Id,
    string Symbol,
    string Name,
    string AssetType,
    DateTime CreatedAt
);

public class ListInstrumentsHandler(IAppDbContext db)
{
    public async Task<IReadOnlyList<ListInstrumentsItem>> HandleAsync(
        CancellationToken cancellationToken
    )
    {
        return await db
            .Instruments.AsNoTracking()
            .OrderBy(i => i.Symbol)
            .Select(i => new ListInstrumentsItem(
                i.Id,
                i.Symbol,
                i.Name,
                i.AssetType,
                i.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
