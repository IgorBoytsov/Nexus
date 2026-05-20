using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitCommandValidator : AbstractValidator<RecoveryViaKeysInitCommand>
    {
        public RecoveryViaKeysInitCommandValidator()
        {
            Include(new LoginValidator());
        }
    }
}