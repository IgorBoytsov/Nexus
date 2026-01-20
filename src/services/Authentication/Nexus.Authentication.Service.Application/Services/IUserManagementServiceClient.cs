using Shared.Contracts.Authentication.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataDto?> GetUserByIdAsync(Guid userId);
        Task<UserAuthDataDto?> GetUserByLoginAsync(string login);
    }
}