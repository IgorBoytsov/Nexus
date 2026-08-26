using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Authentication.Responses;
using Shared.Abstractions.Validations;
using Nexus.Authentication.Service.Application.Abstractions.Validators;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(string Login, string A, string M1) : IRequest<Result<AuthResponse>>, IHasLogin, IHasSrpProof;
}