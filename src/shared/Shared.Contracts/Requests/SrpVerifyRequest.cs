namespace Shared.Contracts.Requests
{
    public sealed record SrpVerifyRequest(string Login, string A, string M1);
}