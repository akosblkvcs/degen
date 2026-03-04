using Degen.Domain.MarketData;
using FluentAssertions;

namespace Degen.Domain.Tests.MarketData;

public class WatchlistTests
{
    [Fact]
    public void Create_ShouldSetNameAndEmptyItems()
    {
        var watchlist = Watchlist.Create("My Crypto");

        watchlist.Name.Should().Be("My Crypto");
        watchlist.Items.Should().BeEmpty();
        watchlist.TenantId.Should().BeNull();
    }

    [Fact]
    public void AddInstrument_ShouldAddItem()
    {
        var watchlist = Watchlist.Create("Favorites");
        var instrumentId = Guid.NewGuid();

        watchlist.AddInstrument(instrumentId);

        watchlist.Items.Should().HaveCount(1);
        watchlist.Items[0].InstrumentId.Should().Be(instrumentId);
        watchlist.Items[0].SortOrder.Should().Be(0);
    }

    [Fact]
    public void AddInstrument_Duplicate_ShouldNotAddTwice()
    {
        var watchlist = Watchlist.Create("Favorites");
        var instrumentId = Guid.NewGuid();

        watchlist.AddInstrument(instrumentId);
        watchlist.AddInstrument(instrumentId);

        watchlist.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveInstrument_ShouldRemoveItem()
    {
        var watchlist = Watchlist.Create("Favorites");
        var instrumentId = Guid.NewGuid();

        watchlist.AddInstrument(instrumentId);
        watchlist.RemoveInstrument(instrumentId);

        watchlist.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveInstrument_NotPresent_ShouldDoNothing()
    {
        var watchlist = Watchlist.Create("Favorites");

        watchlist.RemoveInstrument(Guid.NewGuid());

        watchlist.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddMultiple_ShouldIncrementSortOrder()
    {
        var watchlist = Watchlist.Create("Favorites");

        watchlist.AddInstrument(Guid.NewGuid());
        watchlist.AddInstrument(Guid.NewGuid());
        watchlist.AddInstrument(Guid.NewGuid());

        watchlist.Items.Select(i => i.SortOrder).Should().BeEquivalentTo([0, 1, 2]);
    }

    [Fact]
    public void Rename_ShouldUpdateName()
    {
        var watchlist = Watchlist.Create("Old Name");

        watchlist.Rename("New Name");

        watchlist.Name.Should().Be("New Name");
    }
}
