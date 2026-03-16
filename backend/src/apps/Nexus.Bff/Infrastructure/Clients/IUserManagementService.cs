using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IUserManagementService
    {
        Task<Result> Register(RegisterUserRequest request);
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
    }
}