using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysSet
{
    public sealed class RecoveryViaKeysSetCommandValidator : AbstractValidator<RecoveryViaKeysSetCommand>
    {
        public RecoveryViaKeysSetCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(EncryptedVerifierValidator.Create());
            Include(SrpSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }
    }
}