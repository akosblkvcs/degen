using Degen.Domain.MarketData;
using Degen.Infrastructure.Persistence;
using Degen.Infrastructure.Persistence.Repositories;
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
        // PostgreSQL + TimescaleDB
        services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Default"),
                    npgsql =>
                    {
                        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                        npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    }
                )
                .UseSnakeCaseNamingConvention()
        );

        // Repositories
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<ICandleRepository, CandleRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();

        return services;
    }
}
