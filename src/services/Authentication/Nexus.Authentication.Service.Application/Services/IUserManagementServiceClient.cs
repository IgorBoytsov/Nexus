using Shared.Contracts.Responses;

namespace Nexus.Authentication.Service.Application.Services
{
    public interface IUserManagementServiceClient
    {
        Task<UserAuthDataDto?> GetUserByLoginAsync(string login);
    }
}