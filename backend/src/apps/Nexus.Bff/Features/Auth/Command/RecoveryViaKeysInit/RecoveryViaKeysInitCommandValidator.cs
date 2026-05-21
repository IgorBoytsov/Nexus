using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitCommandValidator : AbstractValidator<RecoveryViaKeysInitCommand>
    {
        public RecoveryViaKeysInitCommandValidator()
        {
            Include(new LoginValidator());
        }
    }
}