using Rebout.Nexus.Contracts.UserManagement.V1;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(UserAuthDataResponse user);
        string GenerateRefreshToken();
    }
}
