using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandValidator : AbstractValidator<ResetPasswordSendCodeCommand>
    {
        public ResetPasswordSendCodeCommandValidator()
        {
            Include(LoginValidator.Create());
        }
    }
}