using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
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