using MediatR;
using Shared.Contracts.Authentication.Responses;
using Quantropic.Toolkit.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse>>;
}