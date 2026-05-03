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
            Include(VerifierValidator.Create());
            Include(ClientSaltValidator.Create());
            Include(EncryptedDekValidator.Create());
            Include(EncryptionAlgorithmValidator.Create());
            Include(IterationsValidator.Create());
            Include(KdfTypeValidator.Create());
        }   
    }
}