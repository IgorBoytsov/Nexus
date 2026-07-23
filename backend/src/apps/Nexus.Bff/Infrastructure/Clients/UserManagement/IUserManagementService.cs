using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public interface IUserManagementService
    {
        Task<Result> Register(RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
        Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId);
        Task<Result> ResetPasswordSendCode(string login);
        Task<Result> ResetPasswordConfirm(string login, string code);
        Task<Result> ResetPasswordComplete(ResetPasswordCompleteRequest request);
        Task<Result> ExistUserByLogin(string login);
        Task<Result<RecoveryViaKeysPayloadResponse>> GetRecoveryKeys(string login);
        Task<Result> RecoveryKeys(RecoveryViaKeysRequest request);
        Task<Result<GetChangePasswordDataResponse>> GetChangePasswordData(GetChangePasswordDataRequest request);
        Task<Result> ChangePassword(ChangePasswordRequest request);
        Task<Result<string>> ChangeAvatar(Stream file, string fileName);
        Task<Result<Unit>> ChangeName(ChangeUserNameRequest request);
        Task<Result<Unit>> DeleteAccountAsync();
    }
}