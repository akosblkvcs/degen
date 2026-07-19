using System.Text.Json;
using Degen.Application.Instruments;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Degen.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AddInstrumentHandler>();
        services.AddScoped<GetInstrumentHandler>();
        services.AddScoped<GetQuoteHandler>();
        services.AddScoped<ListInstrumentsHandler>();

        services.AddValidatorsFromAssemblyContaining<AddInstrumentValidator>();

        // FluentValidation would report the C# name "Symbol", but the API sends and
        // accepts "symbol". This makes validation errors use the same casing as the JSON.
        ValidatorOptions.Global.PropertyNameResolver = (_, member, _) =>
            member is null ? null : JsonNamingPolicy.CamelCase.ConvertName(member.Name);

        return services;
    }
}
