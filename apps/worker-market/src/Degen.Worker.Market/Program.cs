using Degen.Infrastructure;
using Degen.Worker.Market;
using Degen.Worker.Market.Exchanges;
using Degen.Worker.Market.Exchanges.Kraken;
using Degen.Worker.Market.Services;
using Serilog;

// Bootstrap logger for startup errors
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Serilog
    builder.Services.AddSerilog(loggerConfig =>
        loggerConfig.ReadFrom.Configuration(builder.Configuration)
    );

    // Layer registrations
    builder.Services.AddInfrastructure(builder.Configuration);

    // Exchange connection
    builder.Services.AddSingleton<KrakenWebSocketClient>();
    builder.Services.AddSingleton<IExchangeConnection, KrakenConnection>();

    // Services
    builder.Services.AddSingleton<SymbolMapper>();
    builder.Services.AddSingleton<EventPublisher>();
    builder.Services.AddSingleton<CandlePersistenceService>();

    // Background services
    builder.Services.AddHostedService(sp => sp.GetRequiredService<CandlePersistenceService>());
    builder.Services.AddHostedService<MarketDataWorker>();

    using var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
