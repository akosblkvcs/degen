namespace Degen.Domain.MarketData;

public interface IInstrumentRepository
{
    Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Instrument?> GetBySymbolAndExchangeAsync(
        string symbol,
        string exchange,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<Instrument>> GetActiveByExchangeAsync(
        string exchange,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<Instrument>> SearchAsync(string query, CancellationToken ct = default);
    Task AddAsync(Instrument instrument, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
