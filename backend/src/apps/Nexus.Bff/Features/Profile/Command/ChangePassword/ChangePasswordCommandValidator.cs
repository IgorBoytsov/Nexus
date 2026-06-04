using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Profile.Command.ChangePassword
{
    public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            Include(GuidUserIdValidator.Create());
            Include(EncryptedVerifierValidator.Create());
            Include(SrpSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }    
    }
}