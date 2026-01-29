using MediatR;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public class VerifySrpProofHandler(
       IApplicationDbContext context,
       IRedisCacheService redisCacheService,
       IJwtTokenGenerator jwtTokenGenerator,
       IUserManagementServiceClient userManagementClient) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IUserManagementServiceClient _userManagementServiceClient = userManagementClient;

        private readonly BigInteger N = BigInteger.Parse("00AC6BDB41324A9A9BF166DE5E1F403D434A6E1B3B94A7E62AC1211858E002C75AD4455C9D19C0A3180296917A376205164043E20144FF485719D181A99EB574671AC58054457ED444A67032EA17D03AD43464D2397449CA593630A670D90D95A78E846A3C8AF80862098D80F33C42ED7059E75225E0A52718E2379369F65B79680A6560B080092EE71986066735A96A7D42E7597116742B02D3A154471B6A23D84E0D642C790D597A2BB7F5A48F734898BDD138C69493E723491959C1B4BD40C91C1C7924F88D046467A006507E781220A80C55A927906A7C6C9C227E674686DD5D1B855D28F0D604E24586C608630B9A34C4808381A54F0D9080A5F90B60187F", NumberStyles.HexNumber);

        public async Task<Result<AuthResponse>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.A) || string.IsNullOrWhiteSpace(request.M1))
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Validation, "Параметры SRP не могут быть пустыми"));

            var session = await _redisCacheService.GetJsonAsync<SrpSessionState>($"srp_{request.Login}");

            if (session is null)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.NotFound, "Сессия просрочена"));

            BigInteger A = new(Convert.FromBase64String(request.A), isUnsigned: true, isBigEndian: true);
            BigInteger M1_client = new(Convert.FromBase64String(request.M1), isUnsigned: true, isBigEndian: true);
            BigInteger b = new(Convert.FromBase64String(session!.ServerPrivateKeyB), isUnsigned: true, isBigEndian: true);
            BigInteger v = new(Convert.FromBase64String(session.VerifierV), isUnsigned: true, isBigEndian: true);
            BigInteger B = new(Convert.FromBase64String(session.ServerPublicKeyB), isUnsigned: true, isBigEndian: true);

            if (v <= 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Внутренняя ошибка данных: верификатор поврежден"));

            if (A % N == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Не верное значение А"));

            if (A <= 0 || A >= N)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Некорректное значение A (out of range)"));

            BigInteger u = CalculateSrpHash(A, B);

            if (u == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Ошибка вычисления параметра u"));

            BigInteger vU = BigInteger.ModPow(v, u, N);
            BigInteger S = BigInteger.ModPow((A * vU) % N, b, N);

            BigInteger M1_server = CalculateSrpHash(A, B, S);

            if (M1_server != M1_client)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.InvalidPassword, "Неверный логин или пароль"));

            BigInteger M2_server = CalculateSrpHash(A, M1_client, S);

            var userData = await _userManagementServiceClient.GetUserByLoginAsync(request.Login);
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(userData!);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var accessData = AccessData.Create(userData!.Id, refreshToken, accessToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);

            await _context.AccessData.AddAsync(accessData, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _redisCacheService.RemoveAsync($"srp_{request.Login}");
            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, Convert.ToBase64String(M2_server.ToByteArray(true, true))));
        }

        private BigInteger CalculateSrpHash(params BigInteger[] values)
        {
            using var sha256 = SHA256.Create();
            var combinedBytes = new List<byte>();

            foreach (var v in values)
            {
                byte[] b = v.ToByteArray(isUnsigned: true, isBigEndian: true);
                // Если число большое (A, B, S), дополняем до 384 байт. 
                // Если маленькое (M1, u), то до 32 байт.
                int targetLen = b.Length > 32 ? 384 : 32;

                byte[] padded = new byte[targetLen];
                Buffer.BlockCopy(b, 0, padded, targetLen - b.Length, b.Length);
                combinedBytes.AddRange(padded);
            }

            byte[] hash = sha256.ComputeHash(combinedBytes.ToArray());
            return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
        }
    }
}