using System.Text.Json;
using FluentValidation;

namespace Degen.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed");
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var errors = ex
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new
                    {
                        type = "ValidationError",
                        title = "One or more validation errors occurred.",
                        errors,
                    }
                )
            );
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found");
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new { type = "NotFound", title = ex.Message })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new { type = "InternalError", title = "An unexpected error occurred." }
                )
            );
        }
    }
}
