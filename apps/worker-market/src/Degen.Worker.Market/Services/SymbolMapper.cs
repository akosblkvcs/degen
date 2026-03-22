using Degen.Domain.MarketData;

namespace Degen.Worker.Market.Services;

public class SymbolMapper
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SymbolMapper> _logger;
    private volatile IReadOnlyDictionary<string, Instrument> _exchangeSymbolMap =
        new Dictionary<string, Instrument>();

    public SymbolMapper(IServiceScopeFactory scopeFactory, ILogger<SymbolMapper> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task LoadMappingsAsync(string exchange, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var instrumentRepo = scope.ServiceProvider.GetRequiredService<IInstrumentRepository>();

        var instruments = await instrumentRepo.GetActiveByExchangeAsync(exchange, ct);

        var newMap = instruments.ToDictionary(i => i.ExchangeSymbol, i => i);
        _exchangeSymbolMap = newMap;

        LogMessages.SymbolMappingsLoaded(_logger, instruments.Count, exchange);
    }

    public Instrument? Resolve(string exchangeSymbol)
    {
        _exchangeSymbolMap.TryGetValue(exchangeSymbol, out var instrument);

        if (instrument is null)
        {
            var normalized = exchangeSymbol.Replace("/", "");

            _exchangeSymbolMap.TryGetValue(normalized, out instrument);
        }

        return instrument;
    }

    public IReadOnlyList<string> GetSubscriptionSymbols()
    {
        return _exchangeSymbolMap.Keys.ToList();
    }
}
