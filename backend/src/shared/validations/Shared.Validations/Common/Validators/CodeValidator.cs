using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class CodeValidator : AbstractValidator<IHasCode>
    {
        public static CodeValidator Create() => new();

        public CodeValidator()
        {
            RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Пустой код подтверждения")
            .Length(6, 6).WithMessage("Код должен быть ровно 6 символов");
        }
    }
}