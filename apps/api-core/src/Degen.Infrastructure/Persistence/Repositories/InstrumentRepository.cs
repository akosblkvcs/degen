using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;

namespace Degen.Infrastructure.Persistence.Repositories;

public class InstrumentRepository(AppDbContext db) : IInstrumentRepository
{
    public async Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db
            .Instruments.Include(i => i.BaseAsset)
            .Include(i => i.QuoteAsset)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Instrument?> GetBySymbolAndExchangeAsync(
        string symbol,
        string exchange,
        CancellationToken ct = default
    ) =>
        await db
            .Instruments.Include(i => i.BaseAsset)
            .Include(i => i.QuoteAsset)
            .FirstOrDefaultAsync(i => i.Symbol == symbol && i.Exchange == exchange, ct);

    public async Task<IReadOnlyList<Instrument>> GetActiveByExchangeAsync(
        string exchange,
        CancellationToken ct = default
    ) =>
        await db
            .Instruments.Include(i => i.BaseAsset)
            .Include(i => i.QuoteAsset)
            .Where(i => i.Exchange == exchange && i.IsActive)
            .OrderBy(i => i.Symbol)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Instrument>> SearchAsync(
        string query,
        CancellationToken ct = default
    ) =>
        await db
            .Instruments.Include(i => i.BaseAsset)
            .Include(i => i.QuoteAsset)
            .Where(i =>
                i.IsActive
                && (
                    i.Symbol.Contains(query)
                    || i.BaseAsset.Name.Contains(query)
                    || i.BaseAsset.Symbol.Contains(query)
                )
            )
            .OrderBy(i => i.Symbol)
            .Take(20)
            .ToListAsync(ct);

    public async Task AddAsync(Instrument instrument, CancellationToken ct = default) =>
        await db.Instruments.AddAsync(instrument, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
