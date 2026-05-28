using Crossdyne.Toolkit.Results;
using Shared.Contracts;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result> InitPasswordReset(string login)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/v1/users/{login}/password/reset/init", null);
                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ConfirmPasswordReset(string login, ConfirmCodeRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<ConfirmCodeRequest>($"api/v1/users/{login}/password/reset/confirm", request);
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> CompletePasswordReset(RecoveryPasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/v1/users/{request.Login}/password/reset/complete", request);
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result<ChangePasswordInitResponse>> InitPasswordChange(ChangePasswordInitRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{request.UserId}/password/change/init?userId={request.UserId}");
                                                                
                return await HandleResponse<ChangePasswordInitResponse>(response);
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
                var response = await _httpClient.PostAsJsonAsync($"api/v1/users/{request.UserId}/password", request);
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}