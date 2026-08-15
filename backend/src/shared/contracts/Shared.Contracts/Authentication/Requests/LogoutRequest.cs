namespace Shared.Contracts.Authentication.Requests
{
    public sealed record LogoutRequest(string RefreshToken);
}