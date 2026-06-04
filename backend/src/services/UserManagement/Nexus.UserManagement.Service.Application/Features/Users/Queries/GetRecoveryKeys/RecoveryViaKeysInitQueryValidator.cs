using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetRecoveryKeys
{
    public sealed class GetRecoveryKeysQueryValidator : AbstractValidator<GetRecoveryKeysQuery>
    {
        public GetRecoveryKeysQueryValidator()
        {
            Include(new LoginValidator());
        }
    }
}