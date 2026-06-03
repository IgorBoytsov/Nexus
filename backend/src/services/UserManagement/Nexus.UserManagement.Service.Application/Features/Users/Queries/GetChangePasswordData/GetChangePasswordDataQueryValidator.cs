using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetChangePasswordData
{
    public sealed class GetChangePasswordDataQueryValidator : AbstractValidator<GetChangePasswordDataQuery>
    {
        public GetChangePasswordDataQueryValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}