using FluentValidation;
using Nexus.UserManagement.Service.Application.Abstractions.Validators;

namespace Nexus.UserManagement.Service.Application.Validators
{
    public sealed class EncryptedVerifierValidator : AbstractValidator<IHasEncryptedVerifier>
    {
        public static EncryptedVerifierValidator Create() => new();
        
        public EncryptedVerifierValidator()
        {
            RuleFor(x => x.EncryptedVerifier)
            .NotEmpty().WithMessage("Верификатор не может быть пустым.");
        }
    }
}