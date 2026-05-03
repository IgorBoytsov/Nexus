using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class UserNameValidator : AbstractValidator<IHasUserName>
    {
        public static UserNameValidator Create() => new();

        public UserNameValidator()
        {
            RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Имя пользователя нужно обязательно указать.")
            .Length(5, 50).WithMessage("Длинна ника должна быть от 5 до 50 символов.");
        }
    }
}