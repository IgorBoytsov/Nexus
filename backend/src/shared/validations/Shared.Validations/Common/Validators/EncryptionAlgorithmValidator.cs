using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class EncryptionAlgorithmValidator : AbstractValidator<IHasEncryptionAlgorithm>
    {
        public static EncryptionAlgorithmValidator Create() => new();

        public EncryptionAlgorithmValidator()
        {
            RuleFor(x => x.EncryptionAlgorithm)
            .NotEmpty().WithMessage("Метаданные в виде названия алгоритма шифрование должны присутствовать.");
        }
    }
}