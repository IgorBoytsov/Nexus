using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(string Login, string A, string M1) : IRequest<Result<AuthResponse>>, IHasLogin, IHasSrpProof;
}