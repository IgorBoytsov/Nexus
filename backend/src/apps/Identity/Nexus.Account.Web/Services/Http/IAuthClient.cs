using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;

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