namespace Shared.Contracts.Requests
{
    public record RefreshTokenRequest(string AccessToken, string RefreshToken);
}