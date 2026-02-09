using MediatR;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Secure;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;
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

            if (A % SrpConstants.N == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Не верное значение А"));

            if (A <= 0 || A >= SrpConstants.N)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Некорректное значение A (out of range)"));

            BigInteger u = CalculateSrpHash((A, 384), (B, 384));

            if (u == 0)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, "Ошибка вычисления параметра u"));

            BigInteger vU = BigInteger.ModPow(v, u, SrpConstants.N);
            BigInteger S = BigInteger.ModPow((A * vU) % SrpConstants.N, b, SrpConstants.N);

            BigInteger M1_server = CalculateSrpHash((A, 384), (B, 384), (S, 384));

            if (M1_server != M1_client)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.InvalidPassword, "Неверный логин или пароль"));

            BigInteger M2_server = CalculateSrpHash((A, 384), (M1_client, 32), (S, 384));

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
            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, Convert.ToBase64String(ToFixedLength(M2_server, 32))));
        }

        private byte[] ToFixedLength(BigInteger value, int length)
        {
            byte[] bytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);

            if (bytes.Length > length)
                throw new ArgumentException("Значение слишком большое для заданной длины", nameof(value));

            if (bytes.Length == length)
                return bytes;

            byte[] padded = new byte[length];
            Buffer.BlockCopy(bytes, 0, padded, length - bytes.Length, bytes.Length);

            return padded;
        }

        private BigInteger CalculateSrpHash(params (BigInteger value, int length)[] values)
        {
            using var sha256 = SHA256.Create();
            var all = new List<byte>();

            foreach (var (val, len) in values)
                all.AddRange(ToFixedLength(val, len));

            byte[] hash = sha256.ComputeHash(all.ToArray());
            return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
        }
    }
}