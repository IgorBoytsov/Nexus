using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;
using System.Text.Json;

namespace Nexus.Account.Web.Services.Http
{
    public sealed class AuthClient(HttpClient httpClient) : IAuthClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<Result<AuthResponse?>> Login(LoginUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, _jsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result<AuthResponse?>.Success(await response.Content.ReadFromJsonAsync<AuthResponse>());
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result<AuthResponse?>.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));
            }
        }
    }
}