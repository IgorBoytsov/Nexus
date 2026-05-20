using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ExistByLogin
{
    public sealed class ExistUserByLoginCommandValidator : AbstractValidator<ExistUserByLoginCommand>
    {
        public ExistUserByLoginCommandValidator()
        {
            Include(new LoginValidator());
        }
    }
}