namespace Shared.Contracts.Authentication.Requests
{
    public sealed record RefreshTokensRequest(string RefreshToken, string? AccessToken = null);
}