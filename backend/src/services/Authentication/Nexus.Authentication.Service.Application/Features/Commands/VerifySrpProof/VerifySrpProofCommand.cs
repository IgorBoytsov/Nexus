using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(string Login, string A, string M1) : IRequest<Result<AuthResponse>>;
}