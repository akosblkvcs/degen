namespace Degen.Worker.Market.Exchanges;

public interface IExchangeConnection
{
    string ExchangeName { get; }
    Task Disconnected { get; }
    Task ConnectAsync(CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
    Task SubscribeOhlcAsync(string symbol, string interval, CancellationToken ct);
    Task SubscribeTickerAsync(string symbol, CancellationToken ct);
    event EventHandler<CandleReceivedEventArgs> CandleReceived;
    event EventHandler<TickReceivedEventArgs> TickReceived;
}

public enum MarketMessageKind
{
    Snapshot,
    Update,
}

public class CandleReceivedEventArgs(
    string symbol,
    string interval,
    DateTimeOffset timestamp,
    decimal open,
    decimal high,
    decimal low,
    decimal close,
    decimal volume,
    MarketMessageKind kind
) : EventArgs
{
    public string Symbol { get; } = symbol;
    public string Interval { get; } = interval;
    public DateTimeOffset Timestamp { get; } = timestamp;
    public decimal Open { get; } = open;
    public decimal High { get; } = high;
    public decimal Low { get; } = low;
    public decimal Close { get; } = close;
    public decimal Volume { get; } = volume;
    public MarketMessageKind Kind { get; } = kind;
}

public class TickReceivedEventArgs(
    string symbol,
    decimal price,
    decimal volume24h,
    DateTimeOffset timestamp,
    MarketMessageKind kind
) : EventArgs
{
    public string Symbol { get; } = symbol;
    public decimal Price { get; } = price;
    public decimal Volume24h { get; } = volume24h;
    public DateTimeOffset Timestamp { get; } = timestamp;
    public MarketMessageKind Kind { get; } = kind;
}
