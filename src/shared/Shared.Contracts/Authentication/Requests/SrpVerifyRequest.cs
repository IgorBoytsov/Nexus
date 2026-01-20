namespace Shared.Contracts.Authentication.Requests
{
    public sealed record SrpVerifyRequest(string Login, string A, string M1);
}