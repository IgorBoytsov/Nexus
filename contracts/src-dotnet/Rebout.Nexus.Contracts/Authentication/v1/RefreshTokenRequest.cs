namespace Rebout.Nexus.Contracts.Authentication.v1
{
    public record RefreshTokenRequest(string AccessToken, string RefreshToken);
}