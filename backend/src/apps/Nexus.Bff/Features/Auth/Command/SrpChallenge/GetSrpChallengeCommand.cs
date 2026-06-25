using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse?>>, IHasLogin;
}