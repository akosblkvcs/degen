namespace Degen.Domain.MarketData;

public class Candle
{
    public Guid InstrumentId { get; set; }
    public required string Interval { get; set; }
    public DateTime Ts { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}
