using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IUserManagementService
    {
        Task<Result> Register(Shared.Contracts.RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
        Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId);
        Task<Result> SendConfirmCodeEmail(string login);
        Task<Result> ConfirmCodeEmail(string login, string code);
        Task<Result> RecoveryPassword(RecoveryPasswordRequest request);
        Task<Result> ExistUserByLogin(ExistUserBuLoginRequest request);
        Task<Result<RecoveryViaKeysPayloadResponse>> RecoveryViaKeys(RecoveryViaKeysGetPayloadRequest request);
        Task<Result> RecoveryViaKeysSet(RecoveryViaKeysSetRequest request);
        Task<Result<ChangePasswordInitResponse>> ChangePasswordInit(ChangePasswordInitRequest request);
        Task<Result> ChangePassword(ChangePasswordRequest request);
    }
}