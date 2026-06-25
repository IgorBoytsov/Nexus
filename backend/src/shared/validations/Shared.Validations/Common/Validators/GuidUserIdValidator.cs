using FluentValidation;
using Shared.Contracts.Validation.Abstractions;

namespace Shared.Validations.Common.Validators
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