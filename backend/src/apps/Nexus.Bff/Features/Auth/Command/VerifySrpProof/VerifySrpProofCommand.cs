using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(
        string Login,
        string A, 
        string M1) : IRequest<Result<AuthResponse?>>;
}