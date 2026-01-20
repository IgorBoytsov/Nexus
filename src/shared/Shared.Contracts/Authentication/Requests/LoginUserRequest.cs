namespace Shared.Contracts.Authentication.Requests
{
    public sealed record LoginUserRequest(string Password, string Login);
}