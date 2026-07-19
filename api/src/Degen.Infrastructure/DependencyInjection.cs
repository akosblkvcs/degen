using Degen.Application.Common;
using Degen.Application.MarketData;
using Degen.Infrastructure.MarketData;
using Degen.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Degen.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' is not configured."
            );

        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
        );

        services.AddScoped<ICandleStore, CandleStore>();

        var marketDataBaseUrl =
            configuration["MarketData:BaseUrl"]
            ?? throw new InvalidOperationException(
                "'MarketData:BaseUrl' is not configured."
            );

        services
            .AddHttpClient<IPriceProvider, MarketDataApiPriceProvider>(client =>
            {
                client.BaseAddress = new Uri(
                    marketDataBaseUrl.EndsWith('/')
                        ? marketDataBaseUrl
                        : marketDataBaseUrl + "/"
                );
            })
            .AddStandardResilienceHandler();

        services.AddHostedService<DailyCandleRefreshJob>();

        return services;
    }
}
