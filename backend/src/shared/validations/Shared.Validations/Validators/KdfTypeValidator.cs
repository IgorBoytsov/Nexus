using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed  class KdfTypeValidator : AbstractValidator<IHasKdfType>
    {
        public static KdfTypeValidator Create() => new();

        public KdfTypeValidator()
        {
            RuleFor(x => x.KdfType)
            .NotEmpty().WithMessage("KdfType должен быть указан");
        }
    }
}