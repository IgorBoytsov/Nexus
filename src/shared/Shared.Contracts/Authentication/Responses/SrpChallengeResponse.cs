namespace Shared.Contracts.Authentication.Responses
{
    public sealed record SrpChallengeResponse(string Salt, string B);
}