using Shared.Contracts.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataDto?> GetUserByIdAsync(Guid userId);
        Task<UserAuthDataDto?> GetUserByLoginAsync(string login);
    }
}