using FluentValidation;
using Nexus.UserManagement.Service.Application.Abstractions.Validators;

namespace Nexus.UserManagement.Service.Application.Validators
{
    public sealed class SrpSaltValidator : AbstractValidator<IHasSrpSalt>
    {
        public static SrpSaltValidator Create() => new();

        public SrpSaltValidator()
        {
            RuleFor(x => x.SrpSalt)
            .NotEmpty().WithMessage("Клиентская соль не должна быть пустой.");
        }
    }
}