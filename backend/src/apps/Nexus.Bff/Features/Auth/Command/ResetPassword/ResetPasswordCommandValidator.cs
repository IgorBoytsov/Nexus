using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(VerifierValidator.Create());
            Include(ClientSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }
    }
}