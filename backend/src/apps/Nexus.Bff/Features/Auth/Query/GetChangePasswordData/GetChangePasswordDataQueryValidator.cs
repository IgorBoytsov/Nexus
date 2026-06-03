using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Query.GetChangePasswordData
{
    public sealed class GetChangePasswordDataQueryValidator : AbstractValidator<GetChangePasswordDataQuery>
    {
        public GetChangePasswordDataQueryValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}