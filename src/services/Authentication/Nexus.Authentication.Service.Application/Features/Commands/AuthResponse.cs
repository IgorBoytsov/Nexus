namespace Nexus.Authentication.Service.Application.Features.Commands
{
    public record AuthResponse(string AccessToken, string RefreshToken);
}