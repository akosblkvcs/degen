using System.Text;
using System.Text.Json;
using Degen.Contracts.Events;
using RabbitMQ.Client;

namespace Degen.Worker.Market.Services;

public class EventPublisher : IAsyncDisposable
{
    private readonly ILogger<EventPublisher> _logger;
    private readonly IConfiguration _configuration;
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public EventPublisher(ILogger<EventPublisher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync(CancellationToken ct)
    {
        await CleanupAsync();

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "rabbitmq",
            Password = _configuration["RabbitMQ:Password"] ?? "rabbitmq",
            Port = int.TryParse(_configuration["RabbitMQ:Port"], out var port) ? port : 5672,
        };

        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        // Declare exchanges
        await _channel.ExchangeDeclareAsync(
            exchange: "market.data",
            type: "topic",
            durable: true,
            cancellationToken: ct
        );

        LogMessages.RabbitMqInitialized(_logger);
    }

    public async Task PublishCandleUpdateAsync(CandleUpdateEvent evt, CancellationToken ct)
    {
        if (_channel is null)
            return;

        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        var action = evt.IsClosed ? "closed" : "update";
        var routingKey = $"candle.{action}.{evt.Exchange}.{evt.Symbol}.{evt.Interval}";

        await _publishLock.WaitAsync(ct);
        try
        {
            await _channel.BasicPublishAsync(
                exchange: "market.data",
                routingKey: routingKey,
                body: body,
                cancellationToken: ct
            );
        }
        finally
        {
            _publishLock.Release();
        }

        LogMessages.PublishedCandleUpdate(_logger, routingKey);
    }

    public async Task PublishTickAsync(
        string symbol,
        string exchange,
        decimal price,
        decimal volume,
        DateTimeOffset timestamp,
        CancellationToken ct
    )
    {
        if (_channel is null)
            return;

        var evt = new
        {
            Symbol = symbol,
            Exchange = exchange,
            Price = price,
            Volume24h = volume,
            Timestamp = timestamp,
        };
        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        var routingKey = $"tick.{exchange}.{symbol}";

        await _publishLock.WaitAsync(ct);
        try
        {
            await _channel.BasicPublishAsync(
                exchange: "market.data",
                routingKey: routingKey,
                body: body,
                cancellationToken: ct
            );
        }
        finally
        {
            _publishLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await CleanupAsync();

        GC.SuppressFinalize(this);
    }

    private async Task CleanupAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection = null;
        }
    }
}
