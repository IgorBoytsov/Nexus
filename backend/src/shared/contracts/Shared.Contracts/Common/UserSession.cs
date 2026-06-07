namespace Shared.Contracts.Common
{
    public sealed class UserSession(string sessionId, string accessToken, string refreshToken, DateTime accessTokenExpiresAt, string userId)
    {
        public string SessionId { get; set; } = sessionId;
        public string AccessToken { get; set; } = accessToken;
        public string RefreshToken { get; set; } = refreshToken;
        public DateTime AccessTokenExpiresAt { get; set; } = accessTokenExpiresAt;
        public string UserId { get; set; } = userId;
    }
}