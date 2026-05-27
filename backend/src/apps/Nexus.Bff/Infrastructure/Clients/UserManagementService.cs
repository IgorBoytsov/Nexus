using System.Text.Json;
using Nexus.Bff.Features.Profile.Query.Info;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Contracts;
using Microsoft.Extensions.Options;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients
{
    public class UserManagementService(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonOptions) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value;
        
        public async Task<Result> Register(Shared.Contracts.RegisterUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", request, _jsonOptions);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!);
                }

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
                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<PublicEncryptionInfoResponse?>.Failure(errors!);
                }

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
                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<ProfileInfoResponse?>.Failure(errors!)!;
                }

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
                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

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
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

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
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ExistUserByLogin(ExistUserBuLoginRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/exist?login={request.Login}");
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result<RecoveryViaKeysPayloadResponse>> RecoveryViaKeys(RecoveryViaKeysGetPayloadRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/init-recovery-keys?login={request.Login}");
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<RecoveryViaKeysPayloadResponse>.Failure(errors!)!;
                }

                return Result<RecoveryViaKeysPayloadResponse>.Success(await response.Content.ReadFromJsonAsync<RecoveryViaKeysPayloadResponse>(_jsonOptions));
            }
            catch (System.Exception ex)
            {
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> RecoveryViaKeysSet(RecoveryViaKeysSetRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/users/set-recovery-keys", request);
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result<ChangePasswordInitResponse>> ChangePasswordInit(ChangePasswordInitRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/change-password-init?userId={request.UserId}");
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<ChangePasswordInitResponse>.Failure(errors!)!;
                }

                return Result<ChangePasswordInitResponse>.Success(await response.Content.ReadFromJsonAsync<ChangePasswordInitResponse>(_jsonOptions));
            }
            catch (Exception ex)
            {
                return Result<ChangePasswordInitResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/users/change-password", request);
                                                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result.Failure(errors!)!;
                }

                return Result.Success();
            }
            catch (System.Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}