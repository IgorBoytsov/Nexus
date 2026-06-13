using System.Text.Json;
using Crossdyne.Toolkit.Results;
using Microsoft.Extensions.Options;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Kernel.Errors;

namespace Nexus.Bff.Infrastructure.Clients
{
    public sealed class AuthClient(HttpClient client, IOptions<JsonSerializerOptions> jsonOptions) : IAuthClient
    {
        private readonly HttpClient _httpClient = client;
        private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value;
        
        public async Task<Result<SrpChallengeResponse?>> GetSrpChallenge(SrpChallengeRequest request)
        {
            try
            {            
                var response = await _httpClient.PostAsJsonAsync("api/auth/srp/challenge", request, _jsonOptions);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<SrpChallengeResponse>.Failure(errors!)!;
                }
                    
                return Result<SrpChallengeResponse?>.Success(await response.Content.ReadFromJsonAsync<SrpChallengeResponse>(_jsonOptions));
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
                var response = await _httpClient.PostAsJsonAsync("api/auth/srp/verify", request, options: _jsonOptions);
                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<AuthResponse>.Failure(errors!)!;
                }

                return Result<AuthResponse?>.Success(await response.Content.ReadFromJsonAsync<AuthResponse>());
            }
            catch (Exception)
            {
                return Result<AuthResponse?>.Failure(new Error(AppErrors.Api, "Произошла ошибка при верификации"));
            }
        }

        public async Task<Result<Shared.Contracts.Authentication.Responses.AuthResponse>> RefreshTokens(Shared.Contracts.Authentication.Requests.RefreshTokensRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request, options: _jsonOptions);
                                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<Shared.Contracts.Authentication.Responses.AuthResponse>.Failure(errors!)!;
                }

                return Result<Shared.Contracts.Authentication.Responses.AuthResponse?>.Success(await response.Content.ReadFromJsonAsync<Shared.Contracts.Authentication.Responses.AuthResponse>())!;
            }
            catch (Exception ex)
            {
                return Result<Shared.Contracts.Authentication.Responses.AuthResponse?>.Failure(new Error(AppErrors.Api, $"Ошибка обновление токенов: {ex.Message}"))!;
            }
        }

        public async Task<Result<string>> GetPublicKey()
        {
            var response = await _httpClient.GetAsync("api/auth-config/public-key");

            var key = await response.Content.ReadAsStringAsync();

            return Result<string>.Success(key); 
        }
    }
}