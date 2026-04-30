using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Degen.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var pending = await db.Database.GetPendingMigrationsAsync();
            var migrations = pending.ToList();

            if (migrations.Count > 0)
            {
                LogMessages.PendingMigrationsApplying(logger, migrations.Count);

                await db.Database.MigrateAsync();

                LogMessages.DatabaseMigrationsApplied(logger);
            }
            else
            {
                LogMessages.DatabaseUpToDate(logger);
            }
        }
        catch (Exception ex)
        {
            LogMessages.DatabaseMigrationFailed(logger, ex);

            throw;
        }
    }
}
