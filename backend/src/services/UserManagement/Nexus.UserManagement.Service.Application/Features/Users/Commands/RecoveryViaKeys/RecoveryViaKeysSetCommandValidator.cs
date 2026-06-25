using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeys
{
    public sealed class RecoveryViaKeysCommandValidator : AbstractValidator<RecoveryViaKeysCommand>
    {
        public RecoveryViaKeysCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(EncryptedVerifierValidator.Create());
            Include(SrpSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }
    }
}