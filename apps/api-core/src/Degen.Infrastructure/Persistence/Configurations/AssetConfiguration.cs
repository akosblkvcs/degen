using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Degen.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets", "market_data");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Symbol).HasMaxLength(20).IsRequired();

        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();

        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(a => a.Decimals).IsRequired();

        builder.HasIndex(a => a.Symbol).IsUnique();
    }
}
