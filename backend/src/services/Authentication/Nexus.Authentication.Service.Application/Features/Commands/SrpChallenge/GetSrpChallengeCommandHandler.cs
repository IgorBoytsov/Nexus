using MediatR;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using Crossdyne.Security.Configuration;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Extensions;
using Shared.Contracts.Security.Interfaces;
using Shared.Contracts.Authentication.Responses;
using Shared.Contracts.UserManagement.Responses;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Cache.Abstractions;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public class GetSrpChallengeCommandHandler(
        IUserManagementServiceClient userManagementClient,
        ICacheService redisCacheService,
        ISrpServer srpServer,
        ICryptoServices cryptoServices,
        IDataProtector verifierProtector,
        ILogger<GetSrpChallengeCommandHandler> logger) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse>>
    {
        public async Task<Result<SrpChallengeResponse>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            string normalizedLogin = request.Login.Trim().ToLowerInvariant();

            logger.LogDebug("Начало инициализации входа (SrpChallenge) для пользователя: {Login}", normalizedLogin);

            UserAuthDataResponse? userData = await userManagementClient.GetUserByLoginAsync(normalizedLogin);

            if (userData == null)
                return new Error(ErrorCode.NotFound, "Пользователь не найден");

            SrpProfile srpProfile = SrpProfileRegistry.GetProfile((SrpGroup)userData.SrpVersion); 
            var srpContext = SrpContext.FromOptions(srpProfile.Options);

            var encryptedVerifier = userData.EncryptedVerifier;
            var encryptedVerifierWrapKey = userData.EncryptedVerifierWrapKey;
            var verifierWrapKey = verifierProtector.Unprotect(encryptedVerifierWrapKey);
            var verifierWrapKeyBytes = Convert.FromBase64String(verifierWrapKey);
            byte[] saltBytes = Convert.FromBase64String(userData.ClientSalt);

            string? decryptedVerifierBase64 = cryptoServices.DecryptData<string>(encryptedVerifier, verifierWrapKeyBytes);

            byte[] vBytes = Convert.FromBase64String(decryptedVerifierBase64!);

            var sessionState = srpServer.GetSrpChallenge(normalizedLogin, vBytes, saltBytes, srpContext);

            var session = new SrpSessionState(
                normalizedLogin,
                sessionState.PrivateKeyB,
                vBytes,
                sessionState.PublicKeyB,
                saltBytes
            );

            await redisCacheService.SetJsonAsync(RedisKeyExtensions.SrpSession(normalizedLogin), session, TimeSpan.FromMinutes(2));

            logger.LogInformation("SRP challenge успешно сгенерирован для логина: {Login}", normalizedLogin);

            return new SrpChallengeResponse(userData.ClientSalt, Convert.ToBase64String(sessionState.PublicKeyB), userData.SrpVersion);
        }
    }
}