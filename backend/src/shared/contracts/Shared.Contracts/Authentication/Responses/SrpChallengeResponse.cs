namespace Shared.Contracts.Authentication.Responses
{
    public record SrpChallengeResponse(string Salt, string B, int SrpVersion, int SrpCryptoVersion);
}