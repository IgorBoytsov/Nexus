using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed class SrpSaltValidator : AbstractValidator<IHasSrpSalt>
    {
        public static SrpSaltValidator Create() => new();

        public SrpSaltValidator()
        {
            RuleFor(x => x.SrpSalt)
            .NotEmpty().WithMessage("Клиентская соль не должна быть пустой.");
        }
    }
}