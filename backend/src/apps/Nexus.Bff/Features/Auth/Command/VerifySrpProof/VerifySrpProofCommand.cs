using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(
        string Login,
        string A, 
        string M1) : IRequest<Result<VerifierSrpProofDTO>>;
}