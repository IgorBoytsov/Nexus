using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Shared.Contracts.Authentication.Responses;
using Medallion.Threading.Redis;
using Nexus.Authentication.Service.Application.Extensions;
using Medallion.Threading;
using Microsoft.Extensions.Logging;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public sealed class RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IAccessDataRepository accessDataRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration,
        IUserManagementServiceClient userManagementServiceClient,
        RedisDistributedSynchronizationProvider lockProvider,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var lockKey = RedisKeyExtensions.DistributedLock(request.RefreshToken);

            logger.LogDebug("Попытка получить распределенную блокировку для обновления токена");

            await using (await lockProvider.AcquireLockAsync(lockKey, timeout: TimeSpan.FromSeconds(5), cancellationToken))
            {
                logger.LogDebug("Получена распределенная блокировка, обрабатывается токен обновления");

                var maybeStorageToken = await accessDataRepository.GetByAsync(rt => rt.RefreshToken == request.RefreshToken, cancellationToken);

                if (maybeStorageToken.IsNone)
                    return new Error(ErrorCode.Unauthorized, "RefreshToken не найден.");

                var storageToken = maybeStorageToken.Value; 

                if (storageToken == null || storageToken.IsUsed || storageToken.IsRevoked || DateTime.UtcNow > storageToken.ExpiryDate)
                    return new Error(ErrorCode.Unauthorized, "Недействительный или просроченный Refresh токен.");


                if (!string.IsNullOrWhiteSpace(request.AccessToken))
                {
                    var principal = GetPrincipalFromExpiredToken(request.AccessToken);

                    if (principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value is not string userIdFromJwt ||
                        !Guid.TryParse(userIdFromJwt, out var userIdGuid) ||
                        userIdGuid != storageToken.UserId)
                    {
                        return new Error(ErrorCode.Unauthorized, "AccessToken не соответствует Refresh токену.");
                    }
                }

                UserAuthDataResponse? userData = await userManagementServiceClient.GetUserByIdAsync(storageToken.UserId);

                if (userData == null)
                    return new Error(ErrorCode.NotFound, "Пользователь не найден.");

                logger.LogInformation("Генерация новых токенов для пользователя {UserId}", userData.Id);

                var newAccessToken = jwtTokenGenerator.GenerateAccessToken(userData);
                var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

                var newAccessData = AccessData.Create(
                    userId: Guid.Parse(userData.Id),
                    refreshToken: newRefreshToken,
                    accessToken: newAccessToken, 
                    creationDate: DateTime.UtcNow,
                    expiryDate: DateTime.UtcNow.AddDays(30),
                    isUsed: false,
                    isRevoked: false);

                await accessDataRepository.AddAsync(newAccessData, cancellationToken);
                accessDataRepository.Remove(storageToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Успешная генерация новых токенов для пользователя {UserId}", userData.Id);

                return new AuthResponse(newAccessToken, newRefreshToken); 
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка в валидации времени жизни у access токена");
                return null;
            }
        }
    }
}