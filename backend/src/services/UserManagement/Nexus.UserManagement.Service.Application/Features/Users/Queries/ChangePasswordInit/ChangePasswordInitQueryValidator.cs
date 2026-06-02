using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ChangePasswordInit
{
    public sealed class ChangePasswordInitQueryValidator : AbstractValidator<ChangePasswordInitQuery>
    {
        public ChangePasswordInitQueryValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}