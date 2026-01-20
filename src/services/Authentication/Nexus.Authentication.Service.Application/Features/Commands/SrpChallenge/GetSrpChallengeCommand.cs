using MediatR;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse>>;
}