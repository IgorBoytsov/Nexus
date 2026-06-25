using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Bff.Features.Users.Query.ExistUserByLogin
{
    public sealed class ExistUserByLoginCommandValidator : AbstractValidator<ExistUserByLoginQuery>
    {
        public ExistUserByLoginCommandValidator()
        {
            Include(new LoginValidator());
        }
    }
}