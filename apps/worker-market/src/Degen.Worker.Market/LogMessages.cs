namespace Degen.Worker.Market;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Worker started {Exchange}")]
    public static partial void WorkerStarted(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Information, Message = "Worker stopped {Exchange}")]
    public static partial void WorkerStopped(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Error, Message = "Worker failed {Exchange}")]
    public static partial void WorkerFailed(ILogger logger, string exchange, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Worker reconnecting in 5s {Exchange}")]
    public static partial void WorkerReconnecting(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No active instruments {Exchange}")]
    public static partial void InstrumentsEmpty(ILogger logger, string exchange);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Loaded {Count} symbol mappings {Exchange}"
    )]
    public static partial void SymbolMappingsLoaded(ILogger logger, int count, string exchange);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Subscribed to {SymbolCount} symbols, {IntervalCount} intervals {Exchange}"
    )]
    public static partial void SymbolsSubscribed(
        ILogger logger,
        int symbolCount,
        int intervalCount,
        string exchange
    );

    [LoggerMessage(Level = LogLevel.Debug, Message = "Unknown symbol {Symbol} {Exchange}")]
    public static partial void SymbolUnknown(ILogger logger, string exchange, string symbol);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Subscribed OHLC {Symbol} {Interval} {Exchange}"
    )]
    public static partial void OhlcSubscribed(
        ILogger logger,
        string symbol,
        string interval,
        string exchange
    );

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscribed ticker {Symbol} {Exchange}")]
    public static partial void TickerSubscribed(ILogger logger, string symbol, string exchange);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Subscribe failed {Channel}:{Symbol} {Exchange}: {Error}"
    )]
    public static partial void SubscribeFailed(
        ILogger logger,
        string exchange,
        string channel,
        string symbol,
        string error
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Candle processing failed {Symbol} {Exchange}"
    )]
    public static partial void CandleProcessingFailed(
        ILogger logger,
        string exchange,
        string symbol,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Error, Message = "Tick processing failed {Symbol} {Exchange}")]
    public static partial void TickProcessingFailed(
        ILogger logger,
        string exchange,
        string symbol,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Warning, Message = "Domain message parse failed {Exchange}")]
    public static partial void DomainMessageParseFailed(
        ILogger logger,
        string exchange,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Information, Message = "WebSocket connected {Exchange} {Url}")]
    public static partial void WebSocketConnected(ILogger logger, string exchange, string url);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "WebSocket message sent {Exchange}: {Message}"
    )]
    public static partial void WebSocketMessageSent(
        ILogger logger,
        string exchange,
        string message
    );

    [LoggerMessage(Level = LogLevel.Information, Message = "WebSocket disconnected {Exchange}")]
    public static partial void WebSocketDisconnected(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Warning, Message = "WebSocket closed by server {Exchange}")]
    public static partial void WebSocketClosedByServer(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Warning, Message = "WebSocket JSON parse failed {Exchange}")]
    public static partial void WebSocketJsonParseFailed(
        ILogger logger,
        string exchange,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Debug, Message = "WebSocket receive loop cancelled {Exchange}")]
    public static partial void WebSocketReceiveLoopCancelled(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Error, Message = "WebSocket receive loop failed {Exchange}")]
    public static partial void WebSocketReceiveLoopFailed(
        ILogger logger,
        string exchange,
        Exception ex
    );

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "WebSocket error received {Exchange}: {Error}"
    )]
    public static partial void WebSocketErrorReceived(
        ILogger logger,
        string exchange,
        string error
    );

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ publisher initialized")]
    public static partial void RabbitMqPublisherInitialized(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Candle update published {RoutingKey}")]
    public static partial void CandleUpdatePublished(ILogger logger, string routingKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "Persistence service started")]
    public static partial void PersistenceServiceStarted(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Persisted {Count} candles")]
    public static partial void CandlesPersisted(ILogger logger, int count);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Candle persistence failed for {Count} candles"
    )]
    public static partial void CandlePersistenceFailed(ILogger logger, int count, Exception ex);
}
