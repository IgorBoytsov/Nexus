using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed class SrpProofValidator : AbstractValidator<IHasSrpProof>
    {
        public static SrpProofValidator Create() => new();

        public SrpProofValidator()
        {
            RuleFor(x => x.A)
                .NotEmpty().WithMessage("Параметр A обязателен");

            RuleFor(x => x.M1)
                .NotEmpty().WithMessage("Параметр M1 обязателен");
        }
    }
}