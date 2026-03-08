using Shared.Contracts.UserManagement.Responses;
using System.Net.Http.Json;

namespace Nexus.Authentication.Service.Application.Services
{
    public class UserManagementServiceClient(HttpClient httpClient) : IUserManagementServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserAuthDataResponse?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserAuthDataResponse>($"/internal/api/users/by-id/{userId}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserAuthDataResponse?> GetUserByLoginAsync(string login)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserAuthDataResponse>($"/internal/api/users/by-login/{login}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; 
            }
        }
    }
}