using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed class StringUserIdValidator : AbstractValidator<IHasStringUserId>
    {
        public static StringUserIdValidator Create() => new();

        public StringUserIdValidator()
        {
            RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя обязателен.");
        }
    }
}