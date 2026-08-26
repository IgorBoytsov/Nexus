using Microsoft.Extensions.Options;
using Nexus.Authentication.Service.Application.Abstractions.Clients;
using Shared.Contracts.UserManagement.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Nexus.Authentication.Service.Infrastructure.HttpClients
{
    public class UserManagementServiceClient(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonOptions) : IUserManagementServiceClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value;


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
            var response = await _httpClient.GetAsync($"/internal/api/users/by-login/{login}");
            
            if (!response.IsSuccessStatusCode)
            {
                // var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserAuthDataResponse>(_jsonOptions);
        }
    }
}