using FluentValidation;

namespace Degen.Application.MarketData.Queries.GetCandles;

public class GetCandlesValidator : AbstractValidator<GetCandlesQuery>
{
    private static readonly HashSet<string> ValidIntervals = ["1m", "5m", "15m", "1h", "4h", "1d"];

    public GetCandlesValidator()
    {
        RuleFor(x => x.InstrumentId).NotEmpty();

        RuleFor(x => x.Interval)
            .NotEmpty()
            .Must(i => ValidIntervals.Contains(i))
            .WithMessage($"Interval must be one of: {string.Join(", ", ValidIntervals)}");

        RuleFor(x => x.From).LessThan(x => x.To).WithMessage("'From' must be before 'To'");

        RuleFor(x => x.To)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(1))
            .WithMessage("'To' cannot be in the future");
    }
}
