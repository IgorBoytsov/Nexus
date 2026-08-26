using FluentValidation;
using Shared.Abstractions.Validations;

namespace Shared.Validations.Validators
{
    public sealed class LoginValidator : AbstractValidator<IHasLogin>
    {
        public static LoginValidator Create() => new();

        public LoginValidator()
        {
            RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не должен быть пустым.")
            .Length(3, 50).WithMessage("Длинна логина должна быть от 3 до 50 символов.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Логин может содержать только буквы и цифры.");
        }
    }
}