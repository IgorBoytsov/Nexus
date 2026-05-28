using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public interface IUserManagementService
    {
        Task<Result> Register(Shared.Contracts.RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
        Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId);
        Task<Result> InitPasswordReset(string login);
        Task<Result> ConfirmPasswordReset(string login, ConfirmCodeRequest request);
        Task<Result> CompletePasswordReset(RecoveryPasswordRequest request);
        Task<Result> ExistUserByLogin(ExistUserBuLoginRequest request);
        Task<Result<RecoveryViaKeysPayloadResponse>> InitRecoveryKeys(RecoveryViaKeysGetPayloadRequest request);
        Task<Result> SetRecoveryKeys(RecoveryViaKeysSetRequest request);
        Task<Result<ChangePasswordInitResponse>> InitPasswordChange(ChangePasswordInitRequest request);
        Task<Result> ChangePassword(ChangePasswordRequest request);
    }
}