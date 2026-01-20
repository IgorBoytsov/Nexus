using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Authentication.Responses;
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

        public async Task<Result<SrpChallengeResponse>> GetSrpChallenge(SrpChallengeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/srp/challenge", request);
            if (!response.IsSuccessStatusCode) 
                return Result<SrpChallengeResponse>.Failure(new Error(ErrorCode.Server, await response.Content.ReadAsStringAsync()));

            var data = await response.Content.ReadFromJsonAsync<SrpChallengeResponse>();
            return Result<SrpChallengeResponse>.Success(data!);
        }

        public async Task<Result<AuthResponse>> VerifySrpProof(SrpVerifyRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/srp/verify", request);
            if (!response.IsSuccessStatusCode)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, await response.Content.ReadAsStringAsync()));

            var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return Result<AuthResponse>.Success(tokens!);
        }

        private record PublicKeyDto(string PublicKey);

        public async Task<Result<string>> GetPublicKey()
        {
            var response = await _httpClient.GetAsync("api/auth-config/public-key");

            if (!response.IsSuccessStatusCode)
                return Result<string>.Failure(new Error(ErrorCode.Server, await response.Content.ReadAsStringAsync()));

            var data = await response.Content.ReadFromJsonAsync<PublicKeyDto>(_jsonSerializerOptions);

            return Result<string>.Success(data?.PublicKey ?? string.Empty);
        }

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

        public async Task<Result<AuthResponse>> LoginByToken(TokenLoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/token-login", request, _jsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadFromJsonAsync<AuthResponse>();

                return Result<AuthResponse>.Success(responseData!);
            }
            catch (HttpRequestException ex)
            {
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, ex.Message));
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(new Error(ErrorCode.Server, $"Произошла критическая ошибки при отправки запроса: {ex.Message}"));
            }
        }

    }
}