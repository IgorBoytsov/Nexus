using MediatR;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse?>>, IHasLogin;
}