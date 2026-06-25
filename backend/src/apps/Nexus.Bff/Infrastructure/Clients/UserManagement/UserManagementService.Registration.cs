using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result> Register(RegisterUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/users", request, _jsonOptions);
                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result<PublicEncryptionInfoResponse?>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}