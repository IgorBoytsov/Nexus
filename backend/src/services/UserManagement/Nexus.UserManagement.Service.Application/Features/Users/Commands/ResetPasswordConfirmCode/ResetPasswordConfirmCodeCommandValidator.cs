using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode
{
    public sealed class ResetPasswordConfirmCodeCommandValidator : AbstractValidator<ResetPasswordConfirmCodeCommand>
    {
        public ResetPasswordConfirmCodeCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(CodeValidator.Create());
        }
    }
}