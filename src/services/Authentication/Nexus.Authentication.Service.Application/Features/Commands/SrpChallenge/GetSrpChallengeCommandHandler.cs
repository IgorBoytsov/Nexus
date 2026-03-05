using MediatR;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Application.Services;
using Quantropic.Security.Abstractions;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
using Shared.Security.Verifiers;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public class GetSrpChallengeCommandHandler(
        IUserManagementServiceClient userManagementClient,
        IRedisCacheService redisCacheService,
        ISrpServer srpServer,
        IVerifierProtector verifierProtector) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse>>
    {
        private readonly IUserManagementServiceClient _userManagementClient = userManagementClient;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;
        private readonly ISrpServer _srpServer = srpServer;
        private readonly IVerifierProtector _verifierProtector = verifierProtector;

        public async Task<Result<SrpChallengeResponse>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            var userData = await _userManagementClient.GetUserByLoginAsync(request.Login);

            if (userData == null)
                return Result<SrpChallengeResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));

            var decryptedVerifierBase64 = _verifierProtector.Unprotect(userData.PasswordHash);
            byte[] vBytes = Convert.FromBase64String(decryptedVerifierBase64);

            var sessionState = _srpServer.GetSrpChallenge(request.Login, vBytes);

            var session = new SrpSessionState(
                request.Login,
                sessionState.PrivateKeyB,
                Convert.ToBase64String(vBytes),
                sessionState.PublicKeyB
            );

            await _redisCacheService.SetJsonAsync($"srp_{request.Login}", session, TimeSpan.FromMinutes(2));

            return Result<SrpChallengeResponse>.Success(new SrpChallengeResponse(
                userData.ClientSalt,
                sessionState.PublicKeyB
            ));
        }
    }
}