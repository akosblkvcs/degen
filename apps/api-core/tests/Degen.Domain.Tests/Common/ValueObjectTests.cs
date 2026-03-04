using Degen.Domain.Common;
using FluentAssertions;

namespace Degen.Domain.Tests.Common;

public class ValueObjectTests
{
    [Fact]
    public void EqualValues_ShouldBeEqual()
    {
        var a = new TestMoney(100, "USD");
        var b = new TestMoney(100, "USD");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void DifferentValues_ShouldNotBeEqual()
    {
        var a = new TestMoney(100, "USD");
        var b = new TestMoney(200, "USD");

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void CompareWithNull_ShouldNotBeEqual()
    {
        var a = new TestMoney(100, "USD");

        a.Equals(null).Should().BeFalse();
    }

    private class TestMoney(decimal amount, string currency) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return amount;
            yield return currency;
        }
    }
}
