using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(EncryptedVerifierValidator.Create());
            Include(SrpSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }
    }
}