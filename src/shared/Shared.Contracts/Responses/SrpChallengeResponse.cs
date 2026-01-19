namespace Shared.Contracts.Responses
{
    public sealed record SrpChallengeResponse(string Salt, string B);
}