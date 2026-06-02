using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
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