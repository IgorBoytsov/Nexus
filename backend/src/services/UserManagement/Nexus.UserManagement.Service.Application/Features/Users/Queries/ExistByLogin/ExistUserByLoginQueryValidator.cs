using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ExistByLogin
{
    public sealed class ExistUserByLoginQueryValidator : AbstractValidator<ExistUserByLoginQuery>
    {
        public ExistUserByLoginQueryValidator()
        {
            Include(new LoginValidator());
        }
    }
}