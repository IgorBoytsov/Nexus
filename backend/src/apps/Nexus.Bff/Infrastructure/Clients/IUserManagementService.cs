using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IUserManagementService
    {
        Task<Result> Register(RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
        Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId);
        Task<Result> SendConfirmCodeEmail(string login);
        Task<Result> ConfirmCodeEmail(string login, string code);
        Task<Result> RecoveryPassword(RecoveryPasswordRequest request);
    }
}