namespace Degen.Domain.MarketData;

/// <summary>
/// OHLCV candle. Stored in TimescaleDB hypertable.
/// Not an Entity — it's time-series data identified by (instrument_id, interval, timestamp).
/// </summary>
public class Candle
{
    public Guid InstrumentId { get; set; }
    public string Interval { get; set; } = default!; // "1m", "5m", "1h", "1d"
    public DateTimeOffset Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
