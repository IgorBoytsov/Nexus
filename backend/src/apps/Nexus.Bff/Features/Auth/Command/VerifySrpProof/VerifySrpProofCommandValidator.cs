using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
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