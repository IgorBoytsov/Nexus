using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Bff.Features.Auth.Query.GetRecoveryKeys
{
    public sealed class GetRecoveryKeysQueryValidator : AbstractValidator<GetRecoveryKeysQuery>
    {
        public GetRecoveryKeysQueryValidator()
        {
            Include(new LoginValidator());
        }
    }
}