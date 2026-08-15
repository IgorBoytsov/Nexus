using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result> ResetPasswordSendCode(string login)
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

        public async Task<Result> ResetPasswordConfirm(string login, string code)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/v1/users/{login}/password/reset/confirm", code);
                                                                
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }

        public async Task<Result> ResetPasswordComplete(ResetPasswordCompleteRequest request)
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

        public async Task<Result<GetChangePasswordDataResponse>> GetChangePasswordData(GetChangePasswordDataRequest request)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{request.UserId}/password/change?userId={request.UserId}");
                                                                
                return await HandleResponse<GetChangePasswordDataResponse>(response);
            }
            catch (Exception ex)
            {
                return Result<GetChangePasswordDataResponse>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
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