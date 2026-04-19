using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Degen.Infrastructure.Persistence.Configurations;

public class WatchlistConfiguration : IEntityTypeConfiguration<Watchlist>
{
    public void Configure(EntityTypeBuilder<Watchlist> builder)
    {
        builder.ToTable("watchlists", "market_data");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).HasMaxLength(100).IsRequired();
        builder.Property(w => w.TenantId);
        builder
            .HasMany(w => w.Items)
            .WithOne()
            .HasForeignKey(i => i.WatchlistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(w => w.Items).AutoInclude();
    }
}

public class WatchlistItemConfiguration : IEntityTypeConfiguration<WatchlistItem>
{
    public void Configure(EntityTypeBuilder<WatchlistItem> builder)
    {
        builder.ToTable("watchlist_items", "market_data");

        builder.HasKey(i => i.Id);

        builder
            .HasOne(i => i.Instrument)
            .WithMany()
            .HasForeignKey(i => i.InstrumentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(i => i.SortOrder).IsRequired();
    }
}
