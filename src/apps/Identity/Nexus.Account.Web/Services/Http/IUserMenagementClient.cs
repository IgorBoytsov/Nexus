using Quantropic.Toolkit.Results;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Account.Web.Services.Http
{
    public interface IUserMenagementClient
    {
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
    }
}