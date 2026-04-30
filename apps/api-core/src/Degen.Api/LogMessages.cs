using Microsoft.Extensions.Logging;

namespace Degen.Api;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Request validation failed")]
    public static partial void RequestValidationFailed(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Resource not found")]
    public static partial void ResourceNotFound(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception")]
    public static partial void UnhandledException(ILogger logger, Exception ex);
}
