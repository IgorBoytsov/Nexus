
using Shared.Contracts;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(UserAuthDataResponse user);
        string GenerateRefreshToken();
    }
}
