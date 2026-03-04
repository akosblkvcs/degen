using Degen.Domain.Common;
using FluentAssertions;

namespace Degen.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldHaveCorrectState()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldHaveCorrectState()
    {
        var result = Result.Failure("something went wrong");

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("something went wrong");
    }

    [Fact]
    public void GenericSuccess_ShouldContainValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void GenericFailure_ShouldNotContainValue()
    {
        var result = Result.Failure<int>("not found");

        result.IsFailure.Should().BeTrue();
        result.Value.Should().Be(default);
    }

    [Fact]
    public void Success_WithError_ShouldThrow()
    {
        var act = () => new TestableResult(true, "error");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_WithoutError_ShouldThrow()
    {
        var act = () => new TestableResult(false, null);

        act.Should().Throw<InvalidOperationException>();
    }

    private class TestableResult(bool isSuccess, string? error) : Result(isSuccess, error);
}
