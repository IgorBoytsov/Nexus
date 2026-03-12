using MediatR;
using Shared.Contracts.Authentication.Responses;
using Quantropic.Toolkit.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(string Login, string A, string M1) : IRequest<Result<AuthResponse>>;
}