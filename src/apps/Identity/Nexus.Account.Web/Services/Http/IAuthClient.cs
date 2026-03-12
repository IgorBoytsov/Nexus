using Quantropic.Toolkit.Results;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Account.Web.Services.Http
{
    public interface IAuthClient
    {
        Task<Result<SrpChallengeResponse>> GetSrpChallenge(SrpChallengeRequest request);
        Task<Result<AuthResponse>> VerifySrpProof(SrpVerifyRequest request);
        Task<Result<string>> GetPublicKey();
        Task<Result<AuthResponse>> LoginByToken(TokenLoginRequest request);
    }
}