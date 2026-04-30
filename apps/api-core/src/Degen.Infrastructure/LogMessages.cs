using Microsoft.Extensions.Logging;

namespace Degen.Infrastructure;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Applying {Count} pending migrations")]
    public static partial void PendingMigrationsApplying(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database migrations applied")]
    public static partial void DatabaseMigrationsApplied(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database up to date")]
    public static partial void DatabaseUpToDate(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Database migration failed")]
    public static partial void DatabaseMigrationFailed(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeding development data")]
    public static partial void DevelopmentDataSeeding(ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Seeded {AssetCount} assets, {InstrumentCount} instruments, {WatchlistCount} watchlists"
    )]
    public static partial void DevelopmentDataSeeded(
        ILogger logger,
        int assetCount,
        int instrumentCount,
        int watchlistCount
    );
}
