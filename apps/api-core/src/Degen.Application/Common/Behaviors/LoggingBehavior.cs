using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Degen.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        LogMessages.RequestHandling(logger, requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        if (elapsedMs > 500)
        {
            LogMessages.RequestSlow(logger, requestName, elapsedMs);
        }

        LogMessages.RequestHandled(logger, requestName, elapsedMs);

        return response;
    }
}
