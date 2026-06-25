using Crossdyne.Toolkit.Results;
using Nexus.Bff.Features.Profile.Query.Info;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{userId}");
                                
                return await HandleResponse<ProfileInfoResponse>(response);
            }
            catch (Exception ex)
            {
                return Result<ProfileInfoResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ExistUserByLogin(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{login}/exists?login={login}");
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}