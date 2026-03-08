using Shared.Contracts.UserMenagement.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataResponse?> GetUserByIdAsync(Guid userId);
        Task<UserAuthDataResponse?> GetUserByLoginAsync(string login);
    }
}