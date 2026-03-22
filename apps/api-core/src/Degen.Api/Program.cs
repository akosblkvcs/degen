using Degen.Api.Middleware;
using Degen.Application;
using Degen.Infrastructure;
using Degen.Infrastructure.Persistence;
using Serilog;

// Bootstrap logger for startup errors
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Services.AddSerilog(loggerConfig =>
        loggerConfig.ReadFrom.Configuration(builder.Configuration)
    );

    // Layer registrations
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // API services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "Degen API", Version = "v1" });
    });

    // SignalR
    builder.Services.AddSignalR();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "Dev",
            policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        );
    });

    var app = builder.Build();

    // Middleware pipeline
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseCors("Dev");

    if (app.Environment.IsDevelopment())
    {
        await app.InitializeDatabaseAsync();
        await app.SeedDevelopmentDataAsync();

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    // Health check endpoint
    app.MapGet(
        "/healthz",
        () =>
            Results.Ok(
                new
                {
                    status = "healthy",
                    service = "api-core",
                    timestamp = DateTime.UtcNow,
                }
            )
    );

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Required for WebApplicationFactory in integration tests
public partial class Program;
