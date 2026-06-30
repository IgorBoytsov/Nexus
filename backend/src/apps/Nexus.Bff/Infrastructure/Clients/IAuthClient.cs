using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IAuthClient
    {
        Task<Result<SrpChallengeResponse?>> GetSrpChallenge(SrpChallengeRequest request);
        Task<Result<AuthResponse?>> VerifierSrpProof(SrpVerifyRequest request);
        Task<Result<AuthResponse>> RefreshTokens(RefreshTokensRequest request);
        Task<Result<Unit>> Logout(LogoutRequest request);
        Task<Result<string>> GetPublicKey();
    }
}