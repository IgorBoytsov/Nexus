using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
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