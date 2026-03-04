using FluentValidation;

namespace Degen.Application.MarketData.Commands.CreateWatchlist;

public class CreateWatchlistValidator : AbstractValidator<CreateWatchlistCommand>
{
    public CreateWatchlistValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
