using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;

namespace Nexus.Account.Web.Services.Http
{
    public interface IAuthClient
    {
        Task<Result<AuthResponse?>> Login(LoginUserRequest request);
    }
}