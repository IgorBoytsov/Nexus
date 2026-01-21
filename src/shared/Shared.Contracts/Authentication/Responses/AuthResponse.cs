namespace Shared.Contracts.Authentication.Responses
{
    public record AuthResponse(string AccessToken, string RefreshToken, string? M2 = null);
}