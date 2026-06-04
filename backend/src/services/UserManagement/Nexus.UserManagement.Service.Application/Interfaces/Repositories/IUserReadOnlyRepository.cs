using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Interfaces.Repositories
{
    public interface IUserReadOnlyRepository
    {
        Task<Shared.Contracts.UserAuthDataResponse> GetUserByIdAuth(Guid userId);
        Task<Shared.Contracts.UserAuthDataResponse> GetUserByLoginAuth(string login);
        Task<ProfileInfoResponse> GetProfileInfo(Guid userId);
        Task<PublicEncryptionInfoResponse> GetPublicEncryptionInfoResponse(string login);
        Task<GetChangePasswordDataResponse> GetChangePasswordData(Guid userId);
        Task<RecoveryViaKeysPayloadResponse> GetRecoveryKeys(string login);
        Task<bool> ExistUserByLoginAsync(string login);
    }
}