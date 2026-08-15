namespace Rebout.Nexus.Contracts.Authentication.v1
{
    public record AuthResponse(string AccessToken, string RefreshToken, string? M2 = null);
}