using MediatR;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Shared.Contracts.Authentication.Responses;
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
        IUserManagementServiceClient userManagementServiceClient,
        IDistributedLockProvider  lockProvider,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var lockKey = RedisKeyExtensions.DistributedLock(request.RefreshToken);

            logger.LogDebug("Попытка получить распределенную блокировку для обновления токена");

            await using (await lockProvider.AcquireLockAsync(lockKey, timeout: TimeSpan.FromSeconds(5), cancellationToken))
            {
                logger.LogDebug("Получена распределенная блокировка, обрабатывается токен обновления");

                var rtHash = TokenHasher.Hash(request.RefreshToken);
                var maybeStorageToken = await accessDataRepository.GetByAsync(rt => rt.RefreshTokenHash == rtHash, cancellationToken);

                if (maybeStorageToken.IsNone)
                    return new Error(ErrorCode.Unauthorized, "RefreshToken не найден.");

                var storageToken = maybeStorageToken.Value; 

                if (storageToken == null || storageToken.IsUsed || storageToken.IsRevoked || DateTime.UtcNow > storageToken.ExpiryDate)
                    return new Error(ErrorCode.Unauthorized, "Недействительный или просроченный Refresh токен.");

                UserAuthDataResponse? userData = await userManagementServiceClient.GetUserByIdAsync(storageToken.UserId);

                if (userData == null)
                    return new Error(ErrorCode.NotFound, "Пользователь не найден.");

                logger.LogInformation("Генерация новых токенов для пользователя {UserId}", userData.Id);

                var newAccessToken = jwtTokenGenerator.GenerateAccessToken(userData);
                var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

                var newAccessData = AccessData.Create(
                    userId: Guid.Parse(userData.Id),
                    refreshTokenHash: TokenHasher.Hash(newRefreshToken),
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
    }
}