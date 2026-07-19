using Degen.Application.Common;
using Degen.Application.MarketData;
using Degen.Domain.Instruments;
using Degen.Domain.MarketData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Degen.Application.Instruments;

public record AddInstrumentCommand(string Symbol, string Name, string AssetType);

public record AddInstrumentResponse(
    Guid Id,
    string Symbol,
    string Name,
    string AssetType,
    DateTime CreatedAt
);

public abstract record AddInstrumentResult
{
    private AddInstrumentResult() { }

    public sealed record Success(AddInstrumentResponse Instrument) : AddInstrumentResult;

    public sealed record Duplicate(string Symbol) : AddInstrumentResult;

    public sealed record UnknownSymbol(string Symbol) : AddInstrumentResult;
}

public class AddInstrumentHandler(
    IAppDbContext db,
    ICandleStore candleStore,
    IPriceProvider priceProvider,
    ILogger<AddInstrumentHandler> logger
)
{
    public async Task<AddInstrumentResult> HandleAsync(
        AddInstrumentCommand command,
        CancellationToken cancellationToken
    )
    {
        // Symbols are stored uppercase. Validation already guaranteed non-empty fields.
        var symbol = command.Symbol!.Trim().ToUpperInvariant();

        // Cheap pre-check, so an obvious duplicate never costs a provider call.
        if (await SymbolExistsAsync(symbol, cancellationToken))
            return new AddInstrumentResult.Duplicate(symbol);

        // No candles means the provider does not know this symbol: reject before
        // anything is written. Fetched outside the transaction on purpose, so a
        // slow HTTP call never holds one open.
        var candles = await FetchBackfillAsync(symbol, cancellationToken);
        if (candles.Count == 0)
            return new AddInstrumentResult.UnknownSymbol(symbol);

        var instrument = BuildInstrument(command, symbol);

        // Instrument row and 5 years of candles land together or not at all.
        // False means a concurrent request claimed the symbol first.
        if (!await SaveWithCandlesAsync(instrument, candles, cancellationToken))
            return new AddInstrumentResult.Duplicate(symbol);

        return new AddInstrumentResult.Success(
            new(
                instrument.Id,
                instrument.Symbol,
                instrument.Name,
                instrument.AssetType,
                instrument.CreatedAt
            )
        );
    }

    private Task<bool> SymbolExistsAsync(
        string symbol,
        CancellationToken cancellationToken
    ) => db.Instruments.AnyAsync(i => i.Symbol == symbol, cancellationToken);

    private Task<IReadOnlyList<PriceCandle>> FetchBackfillAsync(
        string symbol,
        CancellationToken cancellationToken
    ) =>
        priceProvider.GetCandlesAsync(
            symbol,
            Interval.Daily,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
            cancellationToken
        );

    private static Instrument BuildInstrument(
        AddInstrumentCommand command,
        string symbol
    ) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Symbol = symbol,
            Name = command.Name!.Trim(),
            AssetType = command.AssetType!.Trim().ToLowerInvariant(),
            UserId = "default",
            CreatedAt = DateTime.UtcNow,
        };

    private async Task<bool> SaveWithCandlesAsync(
        Instrument instrument,
        IReadOnlyList<PriceCandle> candles,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await db.BeginTransactionAsync(cancellationToken);

        db.Instruments.Add(instrument);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException)
        {
            return false;
        }

        await candleStore.SaveAsync(
            instrument.Id,
            Interval.Daily,
            candles,
            cancellationToken
        );
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Backfilled {Count} daily candles for {Symbol}",
            candles.Count,
            instrument.Symbol
        );

        return true;
    }
}
