using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandValidator : AbstractValidator<ResetPasswordSendCodeCommand>
    {
        public ResetPasswordSendCodeCommandValidator()
        {
            Include(LoginValidator.Create());
        }
    }
}