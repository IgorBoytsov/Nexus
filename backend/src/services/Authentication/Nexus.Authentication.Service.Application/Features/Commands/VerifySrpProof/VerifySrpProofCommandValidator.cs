using FluentValidation;
using Nexus.Authentication.Service.Application.Validators;
using Shared.Validations.Validators;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed class VerifySrpProofCommandValidator : AbstractValidator<VerifySrpProofCommand>
    {
        public VerifySrpProofCommandValidator()
        {
            Include(LoginValidator.Create());
            Include(SrpProofValidator.Create());
        }
    }
}