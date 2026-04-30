using Degen.Api.Middleware;
using Degen.Application;
using Degen.Infrastructure;
using Degen.Infrastructure.Persistence;
using Serilog;
using Serilog.Events;

// Bootstrap logger captures errors before the host (and full Serilog config) is built.
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(loggerConfig =>
        loggerConfig.ReadFrom.Configuration(builder.Configuration)
    );

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // API services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "Degen API", Version = "v1" });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "Development",
            policy =>
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        );

        options.AddPolicy(
            "Production",
            policy =>
                policy
                    .WithOrigins(
                        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                            ?? []
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        );
    });

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex != null || httpContext.Response.StatusCode >= 500)
                return LogEventLevel.Error;

            if (httpContext.Request.Path.StartsWithSegments("/healthz"))
                return LogEventLevel.Verbose;

            return LogEventLevel.Information;
        };
    });

    if (app.Environment.IsDevelopment())
    {
        // Auto-migrate and seed only in development. Production migrations
        // are applied deliberately via CI.
        await app.InitializeDatabaseAsync();
        await app.SeedDevelopmentDataAsync();

        app.UseCors("Development");
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseCors("Production");
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
