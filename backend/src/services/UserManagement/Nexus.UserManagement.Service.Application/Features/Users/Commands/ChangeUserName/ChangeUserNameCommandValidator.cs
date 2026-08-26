using FluentValidation;
using Nexus.UserManagement.Service.Application.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeUserName
{
    public sealed class ChangeUserNameCommandValidator : AbstractValidator<ChangeUserNameCommand>
    {
        public ChangeUserNameCommandValidator()
        {
            Include(new UserNameValidator());
        }
    }
}