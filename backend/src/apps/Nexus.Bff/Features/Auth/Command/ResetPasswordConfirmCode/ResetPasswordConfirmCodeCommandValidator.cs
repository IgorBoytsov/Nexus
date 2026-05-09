using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
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