using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Query.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitQueryValidator : AbstractValidator<RecoveryViaKeysInitQuery>
    {
        public RecoveryViaKeysInitQueryValidator()
        {
            Include(new LoginValidator());
        }
    }
}