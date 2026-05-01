using System.Text.Json;
using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;

namespace Nexus.Bff.Infrastructure.Clients
{
    public class UserManagementService(HttpClient httpClient) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        
        public async Task<Result> Register(RegisterUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", request, _jsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result<PublicEncryptionInfoResponse?>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/public-encryption-info/{login}");
                response.EnsureSuccessStatusCode();

                return Result<PublicEncryptionInfoResponse?>.Success(await response.Content.ReadFromJsonAsync<PublicEncryptionInfoResponse>());
            }
            catch (Exception ex)
            {
                return Result<PublicEncryptionInfoResponse?>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result<ProfileInfoResponse>> GetProfileInfo(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/profile-info/{userId}");
                response.EnsureSuccessStatusCode();

                return Result<ProfileInfoResponse>.Success(await response.Content.ReadFromJsonAsync<ProfileInfoResponse>());
            }
            catch (System.Exception ex)
            {
                return Result<ProfileInfoResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> SendConfirmCodeEmail(string login)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/recovery-password/send-code/{login}", null);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ConfirmCodeEmail(string login, string code)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/recovery-password/confirm-code/{login}/{code}", null);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> RecoveryPassword(RecoveryPasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/users/recovery-password", request);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}