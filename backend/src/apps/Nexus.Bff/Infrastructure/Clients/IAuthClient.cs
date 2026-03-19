using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IAuthClient
    {
        Task<Result<SrpChallengeResponse?>> GetSrpChallenge(SrpChallengeRequest request);
        Task<Result<AuthResponse?>> VerifierSrpProof(SrpVerifyRequest request);
        Task<Result<string>> GetPublicKey();
    }
}