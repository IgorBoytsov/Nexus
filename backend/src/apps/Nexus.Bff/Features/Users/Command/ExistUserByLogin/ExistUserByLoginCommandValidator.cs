using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Users.Command.ExistUserByLogin
{
    public sealed class ExistUserByLoginCommandValidator : AbstractValidator<ExistUserByLoginCommand>
    {
        public ExistUserByLoginCommandValidator()
        {
            Include(new LoginValidator());
        }
    }
}