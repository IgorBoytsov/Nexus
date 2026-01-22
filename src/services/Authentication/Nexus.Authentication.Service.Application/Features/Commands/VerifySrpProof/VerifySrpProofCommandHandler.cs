using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public class VerifySrpProofHandler(
       IApplicationDbContext context,
       IMemoryCache cache,
       IJwtTokenGenerator jwtTokenGenerator,
       IUserManagementServiceClient userManagementClient) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse>>
    {
        private readonly BigInteger N = BigInteger.Parse("00AC6BDB41324A9A9BF166DE5E1F403D434A6E1B3B94A7E62AC1211858E002C75AD4455C9D19C0A3180296917A376205164043E20144FF485719D181A99EB574671AC58054457ED444A67032EA17D03AD43464D2397449CA593630A670D90D95A78E846A3C8AF80862098D80F33C42ED7059E75225E0A52718E2379369F65B79680A6560B080092EE71986066735A96A7D42E7597116742B02D3A154471B6A23D84E0D642C790D597A2BB7F5A48F734898BDD138C69493E723491959C1B4BD40C91C1C7924F88D046467A006507E781220A80C55A927906A7C6C9C227E674686DD5D1B855D28F0D604E24586C608630B9A34C4808381A54F0D9080A5F90B60187F", NumberStyles.HexNumber);

        public async Task<Result<AuthResponse>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.A) || string.IsNullOrWhiteSpace(request.M1))
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Validation, "Параметры SRP не могут быть пустыми"));

            if (!cache.TryGetValue($"srp_{request.Login}", out SrpSessionState? session))
                return Result<AuthResponse>.Failure(new Error(ErrorCode.NotFound, "Сессия просрочена"));

            BigInteger A = BigInteger.Parse("0" + request.A, NumberStyles.HexNumber);
            BigInteger M1_client = BigInteger.Parse("0" + request.M1, NumberStyles.HexNumber);
            BigInteger b = BigInteger.Parse("0" + session!.ServerPrivateKeyB, NumberStyles.HexNumber);
            BigInteger v = BigInteger.Parse("0" + session.VerifierV, NumberStyles.HexNumber);
            BigInteger B = BigInteger.Parse("0" + session.ServerPublicKeyB, NumberStyles.HexNumber);

            if (v <= 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Внутренняя ошибка данных: верификатор поврежден"));

            if (A % N == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Не верное значение А"));

            if (A <= 0 || A >= N)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Некорректное значение A (out of range)"));

            BigInteger u = CalculateSrpU(A, B);

            if (u == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Ошибка вычисления параметра u"));

            BigInteger vU = BigInteger.ModPow(v, u, N);
            BigInteger baseS = (A * vU) % N;
            BigInteger S = BigInteger.ModPow(baseS, b, N);

            BigInteger M1_server = CalculateSrpM1(A, B, S);

            if (M1_server != M1_client)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.InvalidPassword, "Неверный логин или пароль"));

            BigInteger M2_server = CalculateSrpM2(A, M1_client, S);

            var userData = await userManagementClient.GetUserByLoginAsync(request.Login);
            var accessToken = jwtTokenGenerator.GenerateAccessToken(userData!);
            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

            var accessData = AccessData.Create(userData!.Id, refreshToken, accessToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);

            await context.AccessData.AddAsync(accessData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            cache.Remove($"srp_{request.Login}");
            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, M2_server.ToString("x")));
        }

        private BigInteger CalculateSrpM1(BigInteger A, BigInteger B, BigInteger S)
        {
            string combined = ToHex512(A) + ToHex512(B) + ToHex512(S);
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            return BigInteger.Parse("0" + Convert.ToHexString(hash), NumberStyles.HexNumber);
        }

        private BigInteger CalculateSrpM2(BigInteger A, BigInteger M1, BigInteger S)
        {
            string combined = ToHex512(A) + ToHex512(M1) + ToHex512(S);
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            return BigInteger.Parse("0" + Convert.ToHexString(hash), NumberStyles.HexNumber);
        }

        private BigInteger CalculateSrpU(BigInteger A, BigInteger B)
        {
            string combined = ToHex512(A) + ToHex512(B);
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            return BigInteger.Parse("0" + Convert.ToHexString(hash), NumberStyles.HexNumber);
        }

        private string ToHex512(BigInteger value)
        {
            string hex = value.ToString("x");
            if (hex.Length > 1 && hex.StartsWith("0")) hex = hex.Substring(1);
            return hex.PadLeft(512, '0').ToLower();
        }
    }
}