using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Bff.Features.Auth.Command.CompleteSrpAuth
{
    public sealed record CompleteSrpAuthCommand(string TempAuthToken) : IRequest<Result<CompleteSrpAuthResponse>>;
}