using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Degen.Infrastructure.Persistence.Configurations;

public class InstrumentConfiguration : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable("instruments", "market_data");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Symbol).HasMaxLength(30).IsRequired();
        builder.Property(i => i.ExchangeSymbol).HasMaxLength(30).IsRequired();
        builder.Property(i => i.Exchange).HasMaxLength(30).IsRequired();
        builder.Property(i => i.PriceDecimals).IsRequired();
        builder.Property(i => i.QuantityDecimals).IsRequired();
        builder.Property(i => i.MinOrderSize).HasPrecision(18, 8);
        builder.Property(i => i.IsActive).IsRequired();
        builder
            .HasOne(i => i.BaseAsset)
            .WithMany()
            .HasForeignKey(i => i.BaseAssetId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(i => i.QuoteAsset)
            .WithMany()
            .HasForeignKey(i => i.QuoteAssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.Symbol, i.Exchange }).IsUnique();
        builder.HasIndex(i => new { i.ExchangeSymbol, i.Exchange }).IsUnique();
    }
}
