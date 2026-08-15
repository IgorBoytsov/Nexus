namespace Shared.Contracts.Authentication.Responses
{
    public sealed record CompleteSrpAuthResponse(string SessionId, string UserId, string Login);
}