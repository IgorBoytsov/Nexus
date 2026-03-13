using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.V1;

namespace Nexus.Account.Web.Services.Http
{
    public interface IUserMenagementClient
    {
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
    }
}