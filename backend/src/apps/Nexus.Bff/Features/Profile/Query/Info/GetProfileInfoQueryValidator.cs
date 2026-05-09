using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public sealed class GetProfileInfoQueryValidator : AbstractValidator<GetProfileInfoQuery>
    {
        public GetProfileInfoQueryValidator()
        {
            Include(StringUserIdValidator.Create());
        }    
    }
}