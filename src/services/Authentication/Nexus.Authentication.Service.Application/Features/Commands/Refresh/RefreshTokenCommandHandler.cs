using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Shared.Kernel.Results;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nexus.Authentication.Service.Application.Features.Commands.Refresh
{
    public sealed class RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration,
        IUserManagementServiceClient userManagementServiceClient) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserManagementServiceClient _userManagementServiceClient = userManagementServiceClient;

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);

            if (!Guid.TryParse(principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out var userIdFromJwt))
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Не валидный токен."));
            
            var storageToken = await _context.AccessData.FirstOrDefaultAsync(rt => rt.RefreshToken == request.RefreshToken, cancellationToken);

            if (storageToken == null || storageToken.UserId != userIdFromJwt || storageToken.IsUsed || storageToken.IsRevoked || DateTime.UtcNow > storageToken.ExpiryDate)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Не валидный Refresh токен."));

            storageToken.MarkAsUsed();
            await _context.SaveChangesAsync(cancellationToken);

            var userData = await _userManagementServiceClient.GetUserByIdAsync(storageToken.UserId); 
            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(userData!);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var newAccessData = AccessData.Create(userData!.Id, newRefreshToken, newAccessToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), false, false);
            await _context.AccessData.AddAsync(newAccessData, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
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