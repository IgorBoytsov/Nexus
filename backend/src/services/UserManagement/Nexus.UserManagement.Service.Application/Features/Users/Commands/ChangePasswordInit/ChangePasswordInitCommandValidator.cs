using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePasswordInit
{
    public sealed class ChangePasswordInitCommandValidator : AbstractValidator<ChangePasswordInitCommand>
    {
        public ChangePasswordInitCommandValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}