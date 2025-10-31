using Shared.Contracts.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(UserAuthDataDto user);
        string GenerateRefreshToken();
    }
}
