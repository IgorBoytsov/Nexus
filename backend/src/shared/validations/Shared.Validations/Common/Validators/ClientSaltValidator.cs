using FluentValidation;
using Shared.Validations.Common.Abstractions;

namespace Shared.Validations.Common.Validators
{
    public sealed class ClientSaltValidator : AbstractValidator<IHasClientSalt>
    {
        public static ClientSaltValidator Create() => new();

        public ClientSaltValidator()
        {
            RuleFor(x => x.ClientSalt)
            .NotEmpty().WithMessage("Клиентская соль не должна быть пустой.");
        }
    }
}