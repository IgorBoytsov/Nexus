using System.Text.Json;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;
using Shared.Kernel.Errors;

namespace Nexus.Bff.Infrastructure.Clients
{
 public sealed class AuthClient(HttpClient client) : IAuthClient
    {
        private readonly HttpClient _httpClient = client;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<Result<SrpChallengeResponse?>> GetSrpChallenge(SrpChallengeRequest request)
        {
            try
            {            
                var response = await _httpClient.PostAsJsonAsync("api/auth/srp/challenge", request, _jsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result<SrpChallengeResponse?>.Success(await response.Content.ReadFromJsonAsync<SrpChallengeResponse>());
            }
            catch (Exception ex)
            {
                return Result<SrpChallengeResponse?>.Failure(new Error(AppErrors.Api, $"Произошла ошибка при получение Srp Челленджа: {ex}"));
            }
        }

        public async Task<Result<AuthResponse?>> VerifierSrpProof(SrpVerifyRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/srp/verify", request, _jsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result<AuthResponse?>.Success(await response.Content.ReadFromJsonAsync<AuthResponse>());
            }
            catch (Exception)
            {
                return Result<AuthResponse?>.Failure(new Error(AppErrors.Api, "Произошла ошибка при верификации"));
            }
        }
    }
}