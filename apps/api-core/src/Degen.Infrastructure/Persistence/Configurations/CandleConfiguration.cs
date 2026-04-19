using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Degen.Infrastructure.Persistence.Configurations;

public class CandleConfiguration : IEntityTypeConfiguration<Candle>
{
    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        builder.ToTable("candles", "market_data");

        // Composite primary key
        builder.HasKey(c => new
        {
            c.InstrumentId,
            c.Interval,
            c.Timestamp,
        });

        builder.Property(c => c.Interval).HasMaxLength(5).IsRequired();
        builder.Property(c => c.Timestamp).IsRequired();
        builder.Property(c => c.Open).HasPrecision(18, 8).IsRequired();
        builder.Property(c => c.High).HasPrecision(18, 8).IsRequired();
        builder.Property(c => c.Low).HasPrecision(18, 8).IsRequired();
        builder.Property(c => c.Close).HasPrecision(18, 8).IsRequired();
        builder.Property(c => c.Volume).HasPrecision(18, 8).IsRequired();

        // Index for typical query: instrument + interval + time range
        builder.HasIndex(c => new
        {
            c.InstrumentId,
            c.Interval,
            c.Timestamp,
        });
    }
}
