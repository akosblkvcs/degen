using Degen.Application.Common;
using Degen.Domain.Instruments;
using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Degen.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options),
        IAppDbContext
{
    public DbSet<Instrument> Instruments => Set<Instrument>();
    public DbSet<Candle> Candles => Set<Candle>();

    public async Task<IAppDbTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    ) => new AppDbTransaction(await Database.BeginTransactionAsync(cancellationToken));

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            throw new UniqueConstraintException(ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instrument>(instrument =>
        {
            instrument.Property(i => i.Symbol).HasMaxLength(32);
            instrument.Property(i => i.Name).HasMaxLength(128);
            instrument.Property(i => i.AssetType).HasMaxLength(32);
            instrument.Property(i => i.UserId).HasMaxLength(64);
            instrument.HasIndex(i => new { i.UserId, i.Symbol }).IsUnique();
        });

        modelBuilder.Entity<Candle>(candle =>
        {
            candle.HasKey(c => new
            {
                c.InstrumentId,
                c.Interval,
                c.Ts,
            });
            candle.Property(c => c.Interval).HasMaxLength(8);
            candle.Property(c => c.Open).HasPrecision(28, 12);
            candle.Property(c => c.High).HasPrecision(28, 12);
            candle.Property(c => c.Low).HasPrecision(28, 12);
            candle.Property(c => c.Close).HasPrecision(28, 12);
            candle
                .HasOne<Instrument>()
                .WithMany()
                .HasForeignKey(c => c.InstrumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
