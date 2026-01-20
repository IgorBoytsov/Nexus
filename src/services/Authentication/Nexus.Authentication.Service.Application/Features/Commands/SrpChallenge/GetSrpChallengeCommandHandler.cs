using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Nexus.Authentication.Service.Application.Services;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
using Shared.Security.Verifiers;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    public class GetSrpChallengeCommandHandler(
        IUserManagementServiceClient userManagementClient,
        IMemoryCache cache,
        IVerifierProtector verifierProtector) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse>>
    {
        private readonly IUserManagementServiceClient _userManagementClient = userManagementClient;
        private readonly IMemoryCache _cache = cache;
        private readonly IVerifierProtector _verifierProtector = verifierProtector;

        private readonly BigInteger N = BigInteger.Parse("00AC6BDB41324A9A9BF166DE5E1F403D434A6E1B3B94A7E62AC1211858E002C75AD4455C9D19C0A3180296917A376205164043E20144FF485719D181A99EB574671AC58054457ED444A67032EA17D03AD43464D2397449CA593630A670D90D95A78E846A3C8AF80862098D80F33C42ED7059E75225E0A52718E2379369F65B79680A6560B080092EE71986066735A96A7D42E7597116742B02D3A154471B6A23D84E0D642C790D597A2BB7F5A48F734898BDD138C69493E723491959C1B4BD40C91C1C7924F88D046467A006507E781220A80C55A927906A7C6C9C227E674686DD5D1B855D28F0D604E24586C608630B9A34C4808381A54F0D9080A5F90B60187F", NumberStyles.HexNumber);
        private readonly int g = 2;
        private readonly int k = 3;

        public async Task<Result<SrpChallengeResponse>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            var userData = await _userManagementClient.GetUserByLoginAsync(request.Login);

            if (userData == null)
                return Result<SrpChallengeResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));

            var decryptedVerifier = _verifierProtector.Unprotect(userData.PasswordHash);

            byte[] bBytes = new byte[32];
            RandomNumberGenerator.Fill(bBytes);
            BigInteger b = new(bBytes, true, true);

            BigInteger v = BigInteger.Parse("0" + decryptedVerifier, NumberStyles.HexNumber);
            BigInteger gB = BigInteger.ModPow(g, b, N);
            BigInteger B = (k * v + gB) % N;

            var session = new SrpSessionState(
                request.Login,
                b.ToString("x"),
                decryptedVerifier,
                B.ToString("x")
            );

            _cache.Set($"srp_{request.Login}", session, TimeSpan.FromMinutes(2));

            return Result<SrpChallengeResponse>.Success(new SrpChallengeResponse(userData.ClientSalt, B.ToString("x")));
        }
    }
}