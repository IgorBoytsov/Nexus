using FluentValidation;
using Shared.Abstractions.Validations;

namespace Shared.Validations.Validators
{
    public sealed class GuidUserIdValidator : AbstractValidator<IHasGuidUserId>
    {
        public static GuidUserIdValidator Create() => new();

        public GuidUserIdValidator()
        {
            RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя обязателен.");
        }
    }
}