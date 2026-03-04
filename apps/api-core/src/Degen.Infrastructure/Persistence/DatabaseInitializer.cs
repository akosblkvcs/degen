using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Degen.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applies pending EF Core migrations on startup in development environment.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            var pending = await db.Database.GetPendingMigrationsAsync();
            var migrations = pending.ToList();

            if (migrations.Count > 0)
            {
                logger.LogInformation(
                    "Applying {Count} pending migration(s): {Migrations}",
                    migrations.Count,
                    string.Join(", ", migrations)
                );
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations");
            throw;
        }
    }
}
