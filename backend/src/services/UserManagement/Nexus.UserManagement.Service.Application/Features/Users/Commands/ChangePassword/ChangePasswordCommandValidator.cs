using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            Include(GuidUserIdValidator.Create());
            Include(VerifierValidator.Create());
            Include(ClientSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }    
    }
}