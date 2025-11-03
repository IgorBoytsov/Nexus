using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.LoginByToken
{
    public class TokenLoginCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserManagementServiceClient userManagementServiceClient) : IRequestHandler<TokenLoginCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IUserManagementServiceClient _userManagementServiceClient = userManagementServiceClient;

        public async Task<Result<AuthResponse>> Handle(TokenLoginCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _context.AccessData.FirstOrDefaultAsync(rt => rt.RefreshToken == request.RefreshToken, cancellationToken);

            if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || DateTime.UtcNow > storedToken.ExpiryDate)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Unauthorized, "Недействительный или просроченный токен обновления"));

            storedToken.MarkAsUsed();
            _context.AccessData.Update(storedToken);

            var userData = await _userManagementServiceClient.GetUserByIdAsync(storedToken.UserId);
            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(userData!);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var newAccessData = AccessData.Create(userData!.Id, newRefreshToken, newAccessToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), false, false);
            await _context.AccessData.AddAsync(newAccessData, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
        }
    }
}