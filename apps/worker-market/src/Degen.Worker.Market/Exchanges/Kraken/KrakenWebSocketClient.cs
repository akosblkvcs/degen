using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Degen.Worker.Market.Exchanges.Kraken;

public class KrakenWebSocketClient : IDisposable
{
    private ClientWebSocket? _ws;
    private readonly ILogger<KrakenWebSocketClient> _logger;
    private CancellationTokenSource? _receiveCts;
    private TaskCompletionSource? _disconnected;
    private const string ExchangeName = "Kraken";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public event EventHandler<JsonElement>? MessageReceived;
    public bool IsConnected => _ws?.State == WebSocketState.Open;

    public Task Disconnected => _disconnected?.Task ?? Task.CompletedTask;

    public KrakenWebSocketClient(ILogger<KrakenWebSocketClient> logger)
    {
        _logger = logger;
    }

    public async Task ConnectAsync(string url, CancellationToken ct)
    {
        // Dispose previous connection state before reconnecting
        CleanupConnection();

        var ws = new ClientWebSocket();
        var receiveCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var disconnected = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        _ws = ws;
        _receiveCts = receiveCts;
        _disconnected = disconnected;

        await ws.ConnectAsync(new Uri(url), ct);

        LogMessages.WebSocketConnected(_logger, ExchangeName, url);

        _ = Task.Run(
            async () =>
            {
                try
                {
                    await ReceiveLoopAsync(ws, receiveCts.Token);
                }
                finally
                {
                    disconnected.TrySetResult();
                }
            },
            CancellationToken.None
        );
    }

    public async Task SendAsync(object message, CancellationToken ct)
    {
        var ws = _ws;

        if (ws?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected");

        var json = JsonSerializer.Serialize(message, JsonOptions);

        LogMessages.WebSocketSending(_logger, ExchangeName, json);

        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }

    public async Task DisconnectAsync(CancellationToken ct)
    {
        _receiveCts?.Cancel();

        var ws = _ws;
        if (ws?.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);

            LogMessages.WebSocketDisconnected(_logger, ExchangeName);
        }
    }

    public void Dispose()
    {
        CleanupConnection();
        GC.SuppressFinalize(this);
    }

    private async Task ReceiveLoopAsync(ClientWebSocket ws, CancellationToken ct)
    {
        var buffer = new byte[8192];

        try
        {
            while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await ws.ReceiveAsync(buffer, ct);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    LogMessages.WebSocketClosedByServer(_logger, ExchangeName);

                    break;
                }

                ms.Seek(0, SeekOrigin.Begin);

                try
                {
                    using var doc = await JsonDocument.ParseAsync(ms, cancellationToken: ct);

                    MessageReceived?.Invoke(this, doc.RootElement.Clone());
                }
                catch (JsonException ex)
                {
                    LogMessages.WebSocketMessageParseFailed(_logger, ExchangeName, ex);
                }
            }
        }
        catch (OperationCanceledException)
        {
            LogMessages.WebSocketReceiveLoopCancelled(_logger, ExchangeName);
        }
        catch (Exception ex)
        {
            LogMessages.WebSocketReceiveLoopError(_logger, ExchangeName, ex);
        }
    }

    private void CleanupConnection()
    {
        _receiveCts?.Cancel();
        _receiveCts?.Dispose();
        _receiveCts = null;

        _ws?.Dispose();
        _ws = null;
    }
}
