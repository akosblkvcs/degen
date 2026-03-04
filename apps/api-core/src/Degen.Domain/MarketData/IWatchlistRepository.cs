namespace Degen.Domain.MarketData;

public interface IWatchlistRepository
{
    Task<Watchlist?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Watchlist>> GetAllAsync(
        Guid? tenantId = null,
        CancellationToken ct = default
    );
    Task AddAsync(Watchlist watchlist, CancellationToken ct = default);
    Task DeleteAsync(Watchlist watchlist, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
