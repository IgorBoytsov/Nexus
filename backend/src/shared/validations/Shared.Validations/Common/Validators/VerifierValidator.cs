using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class VerifierValidator : AbstractValidator<IHasVerifier>
    {
        public static VerifierValidator Create() => new();
        
        public VerifierValidator()
        {
            RuleFor(x => x.Verifier)
            .NotEmpty().WithMessage("Верификатор не может быть пустым.");
        }
    }
}