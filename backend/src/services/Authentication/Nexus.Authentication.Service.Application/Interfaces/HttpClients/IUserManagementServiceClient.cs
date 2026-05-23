using Shared.Contracts;

namespace Nexus.Authentication.Service.Application.Interfaces.HttpClients
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataResponse?> GetUserByIdAsync(Guid userId);
        Task<UserAuthDataResponse?> GetUserByLoginAsync(string login);
    }
}