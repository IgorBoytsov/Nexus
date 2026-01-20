using Shared.Contracts.Authentication.Responses;
using System.Net.Http.Json;

namespace Nexus.Authentication.Service.Application.Services
{
    public class UserManagementServiceClient(HttpClient httpClient) : IUserManagementServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserAuthDataDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserAuthDataDto>($"/internal/api/users/by-id/{userId}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserAuthDataDto?> GetUserByLoginAsync(string login)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserAuthDataDto>($"/internal/api/users/by-login/{login}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; 
            }
        }
    }
}