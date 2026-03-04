namespace Degen.Contracts.Events;

/// <summary>
/// Published by worker-market when a candle closes.
/// Consumed by api-core (SignalR relay) and svc-analytics (signal generation).
/// </summary>
public sealed record CandleClosedEvent(
    string Symbol,
    string Exchange,
    string Interval,
    DateTimeOffset Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
);
