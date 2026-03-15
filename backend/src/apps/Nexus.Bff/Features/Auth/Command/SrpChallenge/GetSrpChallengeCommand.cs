using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse?>>;
}