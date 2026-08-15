using FluentValidation;
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