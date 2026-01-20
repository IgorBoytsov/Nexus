using MediatR;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(string Login, string A, string M1) : IRequest<Result<AuthResponse>>;
}