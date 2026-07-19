using Degen.Api.Endpoints;
using Degen.Application;
using Degen.Infrastructure;
using Degen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
        options
            .WithTitle("Degen API")
            .ForceDarkMode()
            .HideSearch()
            .HideSidebar()
            .HideDeveloperTools()
            .DisableAgent()
            .DisableMcp()
            .DisableTelemetry()
    );
}

app.MapHealthChecks("/healthz");
app.MapInstrumentEndpoints();

app.Run();
