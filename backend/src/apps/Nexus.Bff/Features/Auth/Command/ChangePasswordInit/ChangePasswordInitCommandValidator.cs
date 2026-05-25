using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.ChangePasswordInit
{
    public sealed class ChangePasswordInitCommandValidator : AbstractValidator<ChangePasswordInitCommand>
    {
        public ChangePasswordInitCommandValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}