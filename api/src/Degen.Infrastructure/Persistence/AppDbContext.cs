using Degen.Domain.Instruments;
using Microsoft.EntityFrameworkCore;

namespace Degen.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Instrument> Instruments => Set<Instrument>();

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
    }
}
