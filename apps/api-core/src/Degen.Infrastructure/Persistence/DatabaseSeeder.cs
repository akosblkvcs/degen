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

        logger.LogInformation("Seeding development data...");

        // Assets
        var btc = Asset.Create("BTC", "Bitcoin", AssetType.Crypto, 8);
        var eth = Asset.Create("ETH", "Ethereum", AssetType.Crypto, 8);
        var usd = Asset.Create("USD", "US Dollar", AssetType.Fiat, 2);
        var eur = Asset.Create("EUR", "Euro", AssetType.Fiat, 2);
        var sol = Asset.Create("SOL", "Solana", AssetType.Crypto, 8);
        var ada = Asset.Create("ADA", "Cardano", AssetType.Crypto, 6);
        var dot = Asset.Create("DOT", "Polkadot", AssetType.Crypto, 8);

        db.Assets.AddRange(btc, eth, usd, eur, sol, ada, dot);
        await db.SaveChangesAsync();

        // Instruments (Kraken pairs)
        var instruments = new[]
        {
            Instrument.Create("BTC/USD", "BTC/USD", "Kraken", btc.Id, usd.Id, 1, 8, 0.0001m),
            Instrument.Create("ETH/USD", "ETH/USD", "Kraken", eth.Id, usd.Id, 2, 8, 0.005m),
            Instrument.Create("SOL/USD", "SOL/USD", "Kraken", sol.Id, usd.Id, 3, 8, 0.02m),
            Instrument.Create("ADA/USD", "ADA/USD", "Kraken", ada.Id, usd.Id, 6, 6, 5m),
            Instrument.Create("DOT/USD", "DOT/USD", "Kraken", dot.Id, usd.Id, 4, 8, 0.5m),
            Instrument.Create("BTC/EUR", "BTC/EUR", "Kraken", btc.Id, eur.Id, 1, 8, 0.0001m),
            Instrument.Create("ETH/EUR", "ETH/EUR", "Kraken", eth.Id, eur.Id, 2, 8, 0.005m),
        };

        db.Instruments.AddRange(instruments);
        await db.SaveChangesAsync();

        // Default watchlist
        var watchlist = Watchlist.Create("Favorites");
        watchlist.AddInstrument(instruments[0].Id); // BTC/USD
        watchlist.AddInstrument(instruments[1].Id); // ETH/USD
        watchlist.AddInstrument(instruments[2].Id); // SOL/USD

        db.Watchlists.Add(watchlist);
        await db.SaveChangesAsync();

        logger.LogInformation(
            "Seeded {AssetCount} assets, {InstrumentCount} instruments, 1 watchlist",
            7,
            instruments.Length
        );
    }
}
