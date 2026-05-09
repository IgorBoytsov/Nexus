using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class LoginValidator : AbstractValidator<IHasLogin>
    {
        public static LoginValidator Create() => new();

        public LoginValidator()
        {
            RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не должен быть пустым.")
            .Length(5, 50).WithMessage("Длинна логина должна быть от 5 до 50 символов.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Логин может содержать только буквы и цифры.");
        }
    }
}