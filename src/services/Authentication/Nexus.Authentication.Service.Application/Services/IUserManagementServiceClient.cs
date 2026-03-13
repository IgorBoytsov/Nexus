using Rebout.Nexus.Contracts.UserManagement.V1;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataResponse?> GetUserByIdAsync(Guid userId);
        Task<UserAuthDataResponse?> GetUserByLoginAsync(string login);
    }
}