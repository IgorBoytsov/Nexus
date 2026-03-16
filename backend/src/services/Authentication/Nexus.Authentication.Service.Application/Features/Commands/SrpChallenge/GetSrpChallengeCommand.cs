using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse>>;
}