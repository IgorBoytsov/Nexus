using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public sealed class GetSrpChallengeCommandValidator : AbstractValidator<GetSrpChallengeCommand>
    {
        public GetSrpChallengeCommandValidator()
        {
            Include(LoginValidator.Create()); 
        }
    }
}