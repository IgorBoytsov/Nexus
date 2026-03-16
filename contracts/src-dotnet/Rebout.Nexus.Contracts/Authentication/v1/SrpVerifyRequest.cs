namespace Rebout.Nexus.Contracts.Authentication.v1
{
    public record SrpVerifyRequest(string Login, string A, string M1);
}