using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken): IRequest<Result<AuthResponse>>;
}