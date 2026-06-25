using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
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