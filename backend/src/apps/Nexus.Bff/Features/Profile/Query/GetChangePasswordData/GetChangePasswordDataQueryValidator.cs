using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Bff.Features.Profile.Query.GetChangePasswordData
{
    public sealed class GetChangePasswordDataQueryValidator : AbstractValidator<GetChangePasswordDataQuery>
    {
        public GetChangePasswordDataQueryValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}