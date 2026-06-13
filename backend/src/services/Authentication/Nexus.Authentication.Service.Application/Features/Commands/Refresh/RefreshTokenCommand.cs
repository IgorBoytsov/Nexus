using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public sealed record RefreshTokenCommand(string RefreshToken, string? AccessToken = null) : IRequest<Result<AuthResponse>>;
}