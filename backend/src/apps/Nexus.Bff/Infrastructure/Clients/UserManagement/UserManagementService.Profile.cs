using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

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

        public async Task<Result<string>> ChangeAvatar(Stream file, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                if (file.CanSeek)
                    file.Position = 0;

                content.Add(new StreamContent(file), "File", fileName);

                var response = await _httpClient.PatchAsync("api/v1/users/change/avatar", content);

                if (!response.IsSuccessStatusCode)
                    return new Error(ErrorCode.Server, $"HTTP {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return new Error(ErrorCode.Server, $"Ошибка в Api: {ex}");
            }
        }

        public async Task<Result<Unit>> ChangeName(ChangeUserNameRequest request)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync("api/v1/users/change/name", request);

                if (!response.IsSuccessStatusCode)
                    return new Error(ErrorCode.Server, $"HTTP {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

                return Unit.Value;
            }
            catch (Exception ex)
            {
                return new Error(ErrorCode.Server, $"Ошибка в Api: {ex}");
            }
        }

        public async Task<Result<Unit>> DeleteAccountAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/v1/users/account/delete");

                if (!response.IsSuccessStatusCode)
                    return new Error(ErrorCode.Server, $"HTTP {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

                return Unit.Value;
            }
            catch (Exception ex)
            {
                return new Error(ErrorCode.Server, $"Ошибка в Api: {ex}");
            }
        }
    }
}