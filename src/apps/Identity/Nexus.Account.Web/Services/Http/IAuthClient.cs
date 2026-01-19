using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;

namespace Nexus.Account.Web.Services.Http
{
    public interface IAuthClient
    {
        Task<Result<SrpChallengeResponse>> GetSrpChallenge(SrpChallengeRequest request);
        Task<Result<AuthResponse>> VerifySrpProof(SrpVerifyRequest request);
        Task<Result<AuthResponse?>> Login(LoginUserRequest request);
        Task<Result<AuthResponse>> LoginByToken(TokenLoginRequest request);
    }
}