using MediatR;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken): IRequest<Result<AuthResponse>>;
}