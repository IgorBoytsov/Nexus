using MediatR;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.Authentication.Service.Application.Features.Commands.LoginByToken
{
    public class TokenLoginCommandHandler(
        IAccessDataRepository accessDataRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserManagementServiceClient userManagementServiceClient) : IRequestHandler<TokenLoginCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(TokenLoginCommand request, CancellationToken cancellationToken)
        {
            var maybeStoredToken = await accessDataRepository.GetByAsync(rt => rt.RefreshToken == request.RefreshToken, cancellationToken);
  
            var storedToken = maybeStoredToken.Value;

            if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || DateTime.UtcNow > storedToken.ExpiryDate)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Недействительный или просроченный токен обновления"));

            storedToken.MarkAsUsed();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var userData = await userManagementServiceClient.GetUserByIdAsync(storedToken.UserId);
            var newAccessToken = jwtTokenGenerator.GenerateAccessToken(userData!);
            var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

            var newAccessData = AccessData.Create(Guid.Parse(userData!.Id), newRefreshToken, newAccessToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), false, false);
            await accessDataRepository.AddAsync(newAccessData, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
        }
    }
}