using Crossdyne.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result<RecoveryViaKeysPayloadResponse>> GetRecoveryKeys(RecoveryViaKeysGetPayloadRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{request.Login}/recovery-keys?login={request.Login}");
                                                                
                return await HandleResponse<RecoveryViaKeysPayloadResponse>(response);
            }
            catch (Exception ex)
            {
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> RecoveryKeys(RecoveryViaKeysRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/v1/users/{request.Login}/recovery-keys", request);
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}