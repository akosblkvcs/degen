using FluentValidation;

namespace Degen.Api.Filters;

public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(
            model,
            context.HttpContext.RequestAborted
        );
        if (result.IsValid)
        {
            return await next(context);
        }

        return TypedResults.ValidationProblem(result.ToDictionary());
    }
}
