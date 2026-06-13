namespace Shared.Contracts.Authentication.Responses
{
    public sealed record CompleteSrpAuthResponse(string SessionId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, string UserId, string Login);
}