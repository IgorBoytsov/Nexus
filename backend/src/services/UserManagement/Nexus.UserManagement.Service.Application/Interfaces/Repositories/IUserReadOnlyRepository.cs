using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Interfaces.Repositories
{
    public interface IUserReadOnlyRepository
    {
        Task<UserAuthDataResponse> GetUserByIdAuth(Guid userId);
        Task<UserAuthDataResponse> GetUserByLoginAuth(string login);
        Task<ProfileInfoResponse> GetProfileInfo(Guid userId);
        Task<PublicEncryptionInfoResponse> GetPublicEncryptionInfoResponse(string login);
        Task<GetChangePasswordDataResponse> GetChangePasswordData(Guid userId);
        Task<RecoveryViaKeysPayloadResponse> GetRecoveryKeys(string login);
        Task<bool> ExistUserByLoginAsync(string login);
    }
}