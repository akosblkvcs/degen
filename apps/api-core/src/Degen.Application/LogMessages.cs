using Microsoft.Extensions.Logging;

namespace Degen.Application;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling {RequestName}")]
    public static partial void RequestHandling(ILogger logger, string requestName);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Handled {RequestName} in {ElapsedMs}ms"
    )]
    public static partial void RequestHandled(ILogger logger, string requestName, long elapsedMs);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Slow request {RequestName} ({ElapsedMs}ms)"
    )]
    public static partial void RequestSlow(ILogger logger, string requestName, long elapsedMs);
}
