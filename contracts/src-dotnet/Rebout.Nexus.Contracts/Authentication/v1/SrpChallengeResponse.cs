namespace Rebout.Nexus.Contracts.Authentication.v1
{
    public record SrpChallengeResponse(string Salt, string B);
}