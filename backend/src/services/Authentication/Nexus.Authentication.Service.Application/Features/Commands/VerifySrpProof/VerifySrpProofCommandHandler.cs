using MediatR;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Errors;
using Crossdyne.Security.Configuration;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Application.Extensions;
using Shared.Contracts.Authentication.Responses;
using Microsoft.Extensions.Logging;
using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Exceptions;
using Shared.Abstractions.Cache.Abstractions;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public class VerifySrpProofHandler(
        IUnitOfWork unitOfWork,
        IAccessDataRepository accessDataRepository,
        ICacheService redisCacheService,
        IJwtTokenGenerator jwtTokenGenerator,
        ISrpServer srpServer,
        IUserManagementServiceClient userManagementClient,
        ILogger<VerifySrpProofHandler> logger) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            string normalizedLogin = request.Login.Trim().ToLowerInvariant();  
            string srpSessionKey = RedisKeyExtensions.SrpSession(normalizedLogin);

            logger.LogDebug("Начало верификации входа (VerifySrpProof) для пользователя: {Login}", normalizedLogin);
            
            UserAuthDataResponse? userData = await userManagementClient.GetUserByLoginAsync(normalizedLogin);

            if (userData == null)
                return new Error(ErrorCode.NotFound, "Пользователь не найден.");

            if (string.IsNullOrWhiteSpace(request.A) || string.IsNullOrWhiteSpace(request.M1))
                return new Error(AppErrors.Validation, "Неверные параметры аутентификации.");
            
            SrpSessionState? session = await redisCacheService.GetJsonAsync<SrpSessionState>(srpSessionKey);
                
            if (session is null)
            return new Error(AppErrors.SessionExpired, "Сессия аутентификации истекла или недействительна. Повторите вход.");

            var M2_server = srpServer.VerifySrpProof(session, request.A, request.M1, (SrpGroup)userData.SrpVersion);

            var accessToken = jwtTokenGenerator.GenerateAccessToken(userData!);
            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
            
            var accessData = AccessData.Create(
                Guid.Parse(userData!.Id), 
                TokenHasher.Hash(refreshToken),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);
                
            await accessDataRepository.AddAsync(accessData, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await redisCacheService.RemoveAsync(srpSessionKey);

            logger.LogDebug("Успешная верификации входа (VerifySrpProof) для пользователя: {Login}", normalizedLogin);

            return new AuthResponse(accessToken, refreshToken, M2_server);
        }
    }
}