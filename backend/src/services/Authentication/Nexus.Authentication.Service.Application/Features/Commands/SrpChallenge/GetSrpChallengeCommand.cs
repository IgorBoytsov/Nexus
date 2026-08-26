using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Authentication.Responses;
using Shared.Abstractions.Validations;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse>>, IHasLogin;
}