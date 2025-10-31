using MediatR;
using Nexus.Authentication.Service.Application.Services;
using Shared.Kernel.Results;
using Shared.Security.Hasher;

namespace Nexus.Authentication.Service.Application.Features.Commands.Login
{
    public class LoginCommandHandler(
        IUserManagementServiceClient userManagementClient,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserManagementServiceClient _userManagementClient = userManagementClient;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var userData = await _userManagementClient.GetUserByLoginAsync(request.Login);

            if (userData is null)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.NotFound, "Не правильный логин или пароль"));

            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, userData.PasswordHash);

            if (!isPasswordValid)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.NotFound, "Не правильный логин или пароль"));

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(userData);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken));
        }
    }
}