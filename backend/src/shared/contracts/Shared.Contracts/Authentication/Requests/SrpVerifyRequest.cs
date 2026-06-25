namespace Shared.Contracts.Authentication.Requests
{
    public record SrpVerifyRequest(string Login, string A, string M1);
}