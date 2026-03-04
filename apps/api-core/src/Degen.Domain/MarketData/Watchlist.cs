using Degen.Domain.Common;

namespace Degen.Domain.MarketData;

public class Watchlist : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public Guid? TenantId { get; private set; } // null for now, tenant-ready

    private readonly List<WatchlistItem> _items = [];
    public IReadOnlyList<WatchlistItem> Items => _items.AsReadOnly();

    private Watchlist() { } // EF Core

    public static Watchlist Create(string name, Guid? tenantId = null)
    {
        return new Watchlist { Name = name, TenantId = tenantId };
    }

    public void AddInstrument(Guid instrumentId)
    {
        if (_items.Any(i => i.InstrumentId == instrumentId))
            return;

        _items.Add(
            new WatchlistItem
            {
                WatchlistId = Id,
                InstrumentId = instrumentId,
                SortOrder = _items.Count,
            }
        );
    }

    public void RemoveInstrument(Guid instrumentId)
    {
        var item = _items.FirstOrDefault(i => i.InstrumentId == instrumentId);
        if (item is not null)
            _items.Remove(item);
    }

    public void Rename(string name) => Name = name;
}

public class WatchlistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WatchlistId { get; set; }
    public Guid InstrumentId { get; set; }
    public Instrument Instrument { get; set; } = default!;
    public int SortOrder { get; set; }
}
