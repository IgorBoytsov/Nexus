using MediatR;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed record VerifySrpProofCommand(
        string Login,
        string A, 
        string M1) : IRequest<Result<AuthResponse?>>, IHasLogin, IHasSrpProof;
}