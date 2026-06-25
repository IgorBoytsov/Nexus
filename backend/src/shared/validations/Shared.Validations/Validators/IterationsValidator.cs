using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Validators
{
    public sealed class IterationsValidator : AbstractValidator<IHasIterations>
    {
        public static IterationsValidator Create() => new();

        public IterationsValidator()
        {
            RuleFor(x => x.Iterations)
            .GreaterThan(100_000).WithMessage("Кол-во итераций в алгоритме не должно быть меньше 100.000 тыщ .");
        }
    }
}