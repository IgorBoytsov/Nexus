using Shared.Contracts.Authentication.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(UserAuthDataDto user);
        string GenerateRefreshToken();
    }
}
