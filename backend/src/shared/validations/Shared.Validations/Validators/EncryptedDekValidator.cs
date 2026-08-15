using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed class EncryptedDekValidator : AbstractValidator<IHasEncryptedDek>
    {
        public static EncryptedDekValidator Create() => new();

        public EncryptedDekValidator()
        {
            RuleFor(x => x.EncryptedVerifierWrapKey)
            .NotEmpty().WithMessage("Ключ DEK не может быть пустым.");
        }    
    }
}