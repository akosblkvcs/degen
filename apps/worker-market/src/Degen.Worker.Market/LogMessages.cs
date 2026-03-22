using System;
using Microsoft.Extensions.Logging;

namespace Degen.Worker.Market;

public static partial class LogMessages
{
    // ==========================================
    // WORKER LIFECYCLE
    // ==========================================

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Market data worker starting for {Exchange}"
    )]
    public static partial void WorkerStarting(ILogger logger, string exchange);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Market data worker stopping for {Exchange}"
    )]
    public static partial void WorkerStopping(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Exchange} worker encountered an error")]
    public static partial void WorkerError(ILogger logger, string exchange, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Reconnecting to {Exchange} in 5 seconds..."
    )]
    public static partial void WorkerReconnecting(ILogger logger, string exchange);

    // ==========================================
    // SUBSCRIPTIONS & SYMBOLS
    // ==========================================

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No active instruments found for {Exchange}. Waiting..."
    )]
    public static partial void NoInstruments(ILogger logger, string exchange);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Loaded {Count} symbol mappings for {Exchange}"
    )]
    public static partial void SymbolMappingsLoaded(ILogger logger, int count, string exchange);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Subscribed to {SymbolCount} symbols with {IntervalCount} intervals on {Exchange}"
    )]
    public static partial void SubscriptionComplete(
        ILogger logger,
        int symbolCount,
        int intervalCount,
        string exchange
    );

    [LoggerMessage(Level = LogLevel.Debug, Message = "Unknown symbol from {Exchange}: {Symbol}")]
    public static partial void UnknownSymbol(ILogger logger, string exchange, string symbol);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Subscribed to OHLC for {Symbol} ({Interval}) on {Exchange}"
    )]
    public static partial void SubscribedOhlc(
        ILogger logger,
        string symbol,
        string interval,
        string exchange
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Subscribed to ticker for {Symbol} on {Exchange}"
    )]
    public static partial void SubscribedTicker(ILogger logger, string symbol, string exchange);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "{Exchange} subscription error for {Channel}:{Symbol}: {Error}"
    )]
    public static partial void ExchangeSubscribeError(
        ILogger logger,
        string exchange,
        string channel,
        string symbol,
        string error
    );

    // ==========================================
    // PROCESSING
    // ==========================================

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "{Exchange} processing error for candle {Symbol}"
    )]
    public static partial void CandleProcessingError(
        ILogger logger,
        string exchange,
        string symbol,
        Exception ex
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "{Exchange} processing error for tick {Symbol}"
    )]
    public static partial void TickProcessingError(
        ILogger logger,
        string exchange,
        string symbol,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Exchange} parse error for domain message")]
    public static partial void ParseFailed(ILogger logger, string exchange, Exception ex);

    // ==========================================
    // WEBSOCKET I/O
    // ==========================================

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Connected to {Exchange} WebSocket at {Url}"
    )]
    public static partial void WebSocketConnected(ILogger logger, string exchange, string url);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Sending to {Exchange}: {Message}")]
    public static partial void WebSocketSending(ILogger logger, string exchange, string message);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Disconnected from {Exchange} WebSocket"
    )]
    public static partial void WebSocketDisconnected(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Exchange} WebSocket closed by server")]
    public static partial void WebSocketClosedByServer(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Exchange} parse error for WebSocket JSON")]
    public static partial void WebSocketMessageParseFailed(
        ILogger logger,
        string exchange,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Debug, Message = "{Exchange} WebSocket receive loop cancelled")]
    public static partial void WebSocketReceiveLoopCancelled(ILogger logger, string exchange);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Exchange} WebSocket receive loop error")]
    public static partial void WebSocketReceiveLoopError(
        ILogger logger,
        string exchange,
        Exception ex
    );

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Exchange} WebSocket error: {Error}")]
    public static partial void ExchangeError(ILogger logger, string exchange, string error);

    // ==========================================
    // RABBITMQ PUBLISHING
    // ==========================================

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ publisher initialized")]
    public static partial void RabbitMqInitialized(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Published candle update: {RoutingKey}")]
    public static partial void PublishedCandleUpdate(ILogger logger, string routingKey);

    // ==========================================
    // PERSISTENCE
    // ==========================================

    [LoggerMessage(Level = LogLevel.Information, Message = "Candle persistence service started")]
    public static partial void PersistenceServiceStarted(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Persisted {Count} candles")]
    public static partial void CandlesPersisted(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "Persistence error for {Count} candles")]
    public static partial void PersistenceFailed(ILogger logger, int count, Exception ex);
}
