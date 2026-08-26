using FluentValidation;
using Nexus.UserManagement.Service.Application.Abstractions.Validators;

namespace Nexus.UserManagement.Service.Application.Validators
{
    public sealed class UserNameValidator : AbstractValidator<IHasUserName>
    {
        public static UserNameValidator Create() => new();

        public UserNameValidator()
        {
            RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Имя пользователя нужно обязательно указать.")
            .Length(3, 50).WithMessage("Длинна ника должна быть от 2 до 50 символов.");
        }
    }
}