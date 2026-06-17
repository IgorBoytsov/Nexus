using MediatR;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using Shared.Security.Verifiers;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Contracts;
using Crossdyne.Security.Configuration;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Extensions;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public class GetSrpChallengeCommandHandler(
        IUserManagementServiceClient userManagementClient,
        IRedisCacheService redisCacheService,
        ISrpServer srpServer,
        ICryptoServices cryptoServices,
        IVerifierProtector verifierProtector) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse>>
    {
        private readonly IUserManagementServiceClient _userManagementClient = userManagementClient;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;
        private readonly ISrpServer _srpServer = srpServer;
        private readonly ICryptoServices _cryptoServices = cryptoServices;
        private readonly IVerifierProtector _verifierProtector = verifierProtector;

        public async Task<Result<SrpChallengeResponse>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            string normalizedLogin = request.Login.Trim().ToLowerInvariant();
            var userData = await _userManagementClient.GetUserByLoginAsync(normalizedLogin);

            if (userData == null)
                return Result<SrpChallengeResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));

            SrpProfile srpProfile = SrpProfileRegistry.GetProfile((SrpGroup)userData.SrpVersion); 
            var srpContext = SrpContext.FromOptions(srpProfile.Options);

            var cryptoProfile = CryptoProfileRegistry.GetProfile((CryptoVersion)userData.KeyWrapVersion);
            var aesGcmOptions = cryptoProfile.AesGcmOptions;

            var encryptedVerifier = userData.EncryptedVerifier;
            var encryptedVerifierWrapKey = userData.EncryptedVerifierWrapKey;
            var verifierWrapKey = _verifierProtector.Unprotect(encryptedVerifierWrapKey);
            var verifierWrapKeyBytes = Convert.FromBase64String(verifierWrapKey);
            
            var decryptedVerifierBase64 = _cryptoServices.DecryptData<string>(encryptedVerifier, verifierWrapKeyBytes, aesGcmOptions);

            byte[] vBytes = Convert.FromBase64String(decryptedVerifierBase64!);

            var sessionState = _srpServer.GetSrpChallenge(normalizedLogin, vBytes, srpContext);

            var session = new SrpSessionState(
                normalizedLogin,
                sessionState.PrivateKeyB,
                Convert.ToBase64String(vBytes),
                sessionState.PublicKeyB
            );

            await _redisCacheService.SetJsonAsync(RedisKeyExtensions.SrpSession(normalizedLogin), session, TimeSpan.FromMinutes(2));

            return Result<SrpChallengeResponse>.Success(new SrpChallengeResponse(userData.ClientSalt, sessionState.PublicKeyB));
        }
    }
}