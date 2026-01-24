using Shared.Contracts.UserMenagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.Account.Web.Services.Http
{
    public sealed class UserMenagementClient(HttpClient httpClient) : IUserMenagementClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/public-encryption-info/{login}");
                response.EnsureSuccessStatusCode();

                return Result<PublicEncryptionInfoResponse?>.Success(await response.Content.ReadFromJsonAsync<PublicEncryptionInfoResponse>());
            }
            catch (Exception ex)
            {
                return Result<PublicEncryptionInfoResponse?>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}