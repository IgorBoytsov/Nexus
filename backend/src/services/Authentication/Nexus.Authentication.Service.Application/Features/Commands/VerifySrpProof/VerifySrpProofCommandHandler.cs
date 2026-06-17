using MediatR;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Contracts;
using Shared.Kernel.Errors;
using Crossdyne.Security.Configuration;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Application.Extensions;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public class VerifySrpProofHandler(
        IUnitOfWork unitOfWork,
        IAccessDataRepository accessDataRepository,
        IRedisCacheService redisCacheService,
        IJwtTokenGenerator jwtTokenGenerator,
        ISrpServer srpServer,
        IUserManagementServiceClient userManagementClient) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            SrpProfile profile = SrpProfileRegistry.GetProfile(SrpGroup.Rfc5054_3072); 
            var srpContext = SrpContext.FromOptions(profile.Options);

            if (string.IsNullOrWhiteSpace(request.A) || string.IsNullOrWhiteSpace(request.M1))
                return Result<AuthResponse>.Failure(new Error(AppErrors.Validation, "Неверные параметры аутентификации."));

            string normalizedLogin = request.Login.Trim().ToLowerInvariant();  
            string srpSessionKey = RedisKeyExtensions.SrpSession(normalizedLogin);

            var session = await redisCacheService.GetJsonAsync<SrpSessionState>(srpSessionKey);

            if (session is null)
                return Result<AuthResponse>.Failure(new Error(AppErrors.SessionExpired, "Сессия аутентификации истекла или недействительна. Повторите вход."));

            var M2_server = srpServer.VerifySrpProof(session, request.A, request.M1, srpContext);

            var userData = await userManagementClient.GetUserByLoginAsync(normalizedLogin);
            var accessToken = jwtTokenGenerator.GenerateAccessToken(userData!);
            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

            var accessData = AccessData.Create(Guid.Parse(userData!.Id), refreshToken, accessToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);

            await accessDataRepository.AddAsync(accessData, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await redisCacheService.RemoveAsync(srpSessionKey);
            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, M2_server));
        }
    }
}