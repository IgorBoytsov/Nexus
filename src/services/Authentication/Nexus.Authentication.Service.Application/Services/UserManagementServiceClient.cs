using Shared.Contracts.Responses;
using System.Net.Http.Json;

namespace Nexus.Authentication.Service.Application.Services
{
    public class UserManagementServiceClient(HttpClient httpClient) : IUserManagementServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;

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