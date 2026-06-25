using FluentValidation;
using Shared.Validations.Validators;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed class GetSrpChallengeCommandValidator : AbstractValidator<GetSrpChallengeCommand>
    {
        public GetSrpChallengeCommandValidator()
        {
            Include(LoginValidator.Create());
        }
    }
}