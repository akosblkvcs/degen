using Degen.Application.Common;
using Degen.Application.MarketData;
using Microsoft.EntityFrameworkCore;

namespace Degen.Application.Instruments;

public record GetQuoteResponse(
    string Symbol,
    decimal Price,
    decimal? PreviousClose,
    string? Currency
);

public abstract record GetQuoteResult
{
    private GetQuoteResult() { }

    public sealed record Success(GetQuoteResponse Quote) : GetQuoteResult;

    public sealed record InstrumentNotFound : GetQuoteResult;

    public sealed record QuoteUnavailable(string Symbol) : GetQuoteResult;
}

public class GetQuoteHandler(IAppDbContext db, IPriceProvider priceProvider)
{
    public async Task<GetQuoteResult> HandleAsync(
        Guid instrumentId,
        CancellationToken cancellationToken
    )
    {
        // The id only exists to find the symbol; quotes are never stored.
        var instrument = await db
            .Instruments.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == instrumentId, cancellationToken);

        if (instrument is null)
            return new GetQuoteResult.InstrumentNotFound();

        // A tracked instrument the provider has no price for is a different
        // failure from an instrument we never heard of.
        var quote = await priceProvider.GetQuoteAsync(
            instrument.Symbol,
            cancellationToken
        );

        if (quote is null)
            return new GetQuoteResult.QuoteUnavailable(instrument.Symbol);

        return new GetQuoteResult.Success(
            new GetQuoteResponse(
                instrument.Symbol,
                quote.Price,
                quote.PreviousClose,
                quote.Currency
            )
        );
    }
}
