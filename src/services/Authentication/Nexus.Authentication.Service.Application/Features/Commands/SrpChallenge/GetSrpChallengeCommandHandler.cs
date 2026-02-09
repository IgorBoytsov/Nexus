using MediatR;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Application.Secure;
using Nexus.Authentication.Service.Application.Services;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
using Shared.Security.Verifiers;
using System.Numerics;
using System.Security.Cryptography;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public class GetSrpChallengeCommandHandler(
        IUserManagementServiceClient userManagementClient,
        IRedisCacheService redisCacheService,
        IVerifierProtector verifierProtector) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse>>
    {
        private readonly IUserManagementServiceClient _userManagementClient = userManagementClient;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;
        private readonly IVerifierProtector _verifierProtector = verifierProtector;

        public async Task<Result<SrpChallengeResponse>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            var userData = await _userManagementClient.GetUserByLoginAsync(request.Login);

            if (userData == null)
                return Result<SrpChallengeResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));

            var decryptedVerifierBase64 = _verifierProtector.Unprotect(userData.PasswordHash);
            byte[] vBytes = Convert.FromBase64String(decryptedVerifierBase64);
            BigInteger v = new(vBytes, isUnsigned: true, isBigEndian: true);

            byte[] bBytes = new byte[32];
            RandomNumberGenerator.Fill(bBytes);
            BigInteger b = new(bBytes, isUnsigned: true, isBigEndian: true);

            BigInteger gB = BigInteger.ModPow(SrpConstants.g, b, SrpConstants.N);
            BigInteger B = (SrpConstants.k * v + gB) % SrpConstants.N;

            var session = new SrpSessionState(
                request.Login,
                Convert.ToBase64String(bBytes),
                Convert.ToBase64String(vBytes),
                Convert.ToBase64String(B.ToByteArray(isUnsigned: true, isBigEndian: true))
            );

            await _redisCacheService.SetJsonAsync($"srp_{request.Login}", session, TimeSpan.FromMinutes(2));

            return Result<SrpChallengeResponse>.Success(new SrpChallengeResponse(
                userData.ClientSalt,
                session.ServerPublicKeyB
            ));
        }
    }
}