namespace Degen.Contracts.Events;

/// <summary>
/// Published on every OHLC update from the exchange (interim and close).
/// </summary>
public sealed record CandleUpdateEvent(
    string Symbol,
    string Exchange,
    string Interval,
    DateTimeOffset Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    bool IsClosed
);
