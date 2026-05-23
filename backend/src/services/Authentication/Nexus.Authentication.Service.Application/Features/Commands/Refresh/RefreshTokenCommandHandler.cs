using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public sealed class RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IAccessDataRepository accessDataRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration,
        IUserManagementServiceClient userManagementServiceClient) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);

            if (!Guid.TryParse(principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out var userIdFromJwt))
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Не валидный токен."));
            
            var maybeStorageToken = await accessDataRepository.GetByAsync(rt => rt.RefreshToken == request.RefreshToken, cancellationToken);

            var storageToken = maybeStorageToken.Value;

            if (storageToken == null || storageToken.UserId != userIdFromJwt || storageToken.IsUsed || storageToken.IsRevoked || DateTime.UtcNow > storageToken.ExpiryDate)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Не валидный Refresh токен."));

            storageToken.MarkAsUsed();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var userData = await userManagementServiceClient.GetUserByIdAsync(storageToken.UserId); 
            var newAccessToken = jwtTokenGenerator.GenerateAccessToken(userData!);
            var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

            var newAccessData = AccessData.Create(Guid.Parse(userData!.Id), newRefreshToken, newAccessToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), false, false);
            await accessDataRepository.AddAsync(newAccessData, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
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

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    Debug.WriteLine("ОШИБКА ВАЛИДАЦИИ: Токен не является JwtSecurityToken или алгоритм не HmacSha256.");
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("==========================================================");
                Debug.WriteLine("ОШИБКА ВАЛИДАЦИИ ТОКЕНА:");
                Debug.WriteLine($"ТИП ОШИБКИ: {ex.GetType().Name}");
                Debug.WriteLine($"СООБЩЕНИЕ: {ex.Message}");
                Debug.WriteLine("ПОЛНЫЙ СТЕК ОШИБКИ:");
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine("==========================================================");

                return null;
            }
        }
    }
}