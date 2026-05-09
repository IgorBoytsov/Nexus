using MediatR;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public sealed record GetSrpChallengeCommand(string Login) : IRequest<Result<SrpChallengeResponse>>, IHasLogin;
}