using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
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