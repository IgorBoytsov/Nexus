using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitQueryValidator : AbstractValidator<RecoveryViaKeysInitQuery>
    {
        public RecoveryViaKeysInitQueryValidator()
        {
            Include(new LoginValidator());
        }
    }
}