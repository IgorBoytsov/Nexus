using Shared.Contracts.UserMenagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.Account.Web.Services.Http
{
    public interface IUserMenagementClient
    {
        Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login);
    }
}