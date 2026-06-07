using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Contracts.Authentication.Requests;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IAuthClient
    {
        Task<Result<SrpChallengeResponse?>> GetSrpChallenge(SrpChallengeRequest request);
        Task<Result<AuthResponse?>> VerifierSrpProof(SrpVerifyRequest request);
        Task<Result<Shared.Contracts.Authentication.Responses.AuthResponse>> RefreshTokens(RefreshTokensRequest request);
        Task<Result<string>> GetPublicKey();
    }
}