using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;

namespace Degen.Infrastructure.Persistence.Repositories;

public class WatchlistRepository(AppDbContext db) : IWatchlistRepository
{
    public async Task<Watchlist?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db
            .Watchlists.Include(w => w.Items)
                .ThenInclude(i => i.Instrument)
                    .ThenInclude(i => i.BaseAsset)
            .Include(w => w.Items)
                .ThenInclude(i => i.Instrument)
                    .ThenInclude(i => i.QuoteAsset)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IReadOnlyList<Watchlist>> GetAllAsync(
        Guid? tenantId = null,
        CancellationToken ct = default
    ) =>
        await db.Watchlists.Where(w => w.TenantId == tenantId).OrderBy(w => w.Name).ToListAsync(ct);

    public async Task AddAsync(Watchlist watchlist, CancellationToken ct = default) =>
        await db.Watchlists.AddAsync(watchlist, ct);

    public async Task DeleteAsync(Watchlist watchlist, CancellationToken ct = default)
    {
        db.Watchlists.Remove(watchlist);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
