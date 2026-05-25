using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(UserNameValidator.Create());
            Include(EncryptedVerifierValidator.Create());
            Include(SrpSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
        }   
    }
}