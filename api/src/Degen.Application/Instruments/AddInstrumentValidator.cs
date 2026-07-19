using FluentValidation;

namespace Degen.Application.Instruments;

public class AddInstrumentValidator : AbstractValidator<AddInstrumentCommand>
{
    public AddInstrumentValidator()
    {
        RuleFor(c => c.Symbol)
            .NotEmpty()
            .WithMessage("Symbol is required.")
            .MaximumLength(32);

        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(128);

        RuleFor(c => c.AssetType)
            .NotEmpty()
            .WithMessage("AssetType is required.")
            .MaximumLength(32);
    }
}
