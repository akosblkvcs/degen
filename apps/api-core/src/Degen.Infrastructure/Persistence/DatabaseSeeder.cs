using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Degen.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedDevelopmentDataAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        // Skip if already seeded
        if (await db.Assets.AnyAsync())
            return;

        LogMessages.DevelopmentDataSeeding(logger);

        // Assets
        var btc = Asset.Create("BTC", "Bitcoin", AssetType.Crypto, 8);
        var eth = Asset.Create("ETH", "Ethereum", AssetType.Crypto, 8);
        var usd = Asset.Create("USD", "US Dollar", AssetType.Fiat, 2);
        var eur = Asset.Create("EUR", "Euro", AssetType.Fiat, 2);

        db.Assets.AddRange(btc, eth, usd, eur);

        await db.SaveChangesAsync();

        // Instruments (Kraken pairs)
        var instruments = new[]
        {
            Instrument.Create("BTC/USD", "BTC/USD", "Kraken", btc.Id, usd.Id, 1, 8, 0.0001m),
            Instrument.Create("BTC/EUR", "BTC/EUR", "Kraken", btc.Id, eur.Id, 1, 8, 0.0001m),
            Instrument.Create("ETH/USD", "ETH/USD", "Kraken", eth.Id, usd.Id, 2, 8, 0.005m),
            Instrument.Create("ETH/EUR", "ETH/EUR", "Kraken", eth.Id, eur.Id, 2, 8, 0.005m),
        };

        db.Instruments.AddRange(instruments);

        await db.SaveChangesAsync();

        // Default watchlist
        var watchlist = Watchlist.Create("Favorites");

        watchlist.AddInstrument(instruments[1].Id); // ETH/USD

        db.Watchlists.Add(watchlist);

        await db.SaveChangesAsync();

        LogMessages.DevelopmentDataSeeded(logger, 4, 4, 1);
    }
}
