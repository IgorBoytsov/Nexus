using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public interface IUserManagementService
    {
        Task<Result> Register(Shared.Contracts.UserManagement.Requests.RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
        Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId);
        Task<Result> ResetPasswordSendCode(string login);
        Task<Result> ResetPasswordConfirm(string login, ResetPasswordConfirmCodeRequest request);
        Task<Result> ResetPasswordComplete(ResetPasswordCompleteRequest request);
        Task<Result> ExistUserByLogin(string login);
        Task<Result<RecoveryViaKeysPayloadResponse>> GetRecoveryKeys(string login);
        Task<Result> RecoveryKeys(RecoveryViaKeysRequest request);
        Task<Result<GetChangePasswordDataResponse>> GetChangePasswordData(GetChangePasswordDataRequest request);
        Task<Result> ChangePassword(ChangePasswordRequest request);
    }
}