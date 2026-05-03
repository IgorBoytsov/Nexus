using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class EncryptedDekValidator : AbstractValidator<IHasEncryptedDek>
    {
        public static EncryptedDekValidator Create() => new();

        public EncryptedDekValidator()
        {
            RuleFor(x => x.EncryptedDek)
            .NotEmpty().WithMessage("Ключ DEK не может быть пустым.");
        }    
    }
}