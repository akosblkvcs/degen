using Degen.Domain.Instruments;
using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;

namespace Degen.Application.Common;

public interface IAppDbContext
{
    DbSet<Instrument> Instruments { get; }
    DbSet<Candle> Candles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IAppDbTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    );
}
