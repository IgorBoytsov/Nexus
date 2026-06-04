using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService
    {
        public async Task<Result<PublicEncryptionInfoResponse?>> GetPublicEncryptionInfo(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v1/users/{login}/crypto/public");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
                    return Result<PublicEncryptionInfoResponse?>.Failure(errors!);
                }

                return Result<PublicEncryptionInfoResponse?>.Success(await response.Content.ReadFromJsonAsync<PublicEncryptionInfoResponse>());
            }
            catch (Exception ex)
            {
                return Result<PublicEncryptionInfoResponse?>.Failure(new Error(ErrorCode.Server, $"Ошибка в Api: {ex}"));
            }
        }
    }
}