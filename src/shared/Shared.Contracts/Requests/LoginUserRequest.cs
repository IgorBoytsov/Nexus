namespace Shared.Contracts.Requests
{
    public sealed record LoginUserRequest(string Password, string Login, string Email);
}