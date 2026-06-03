using System.Text.Json;
using Crossdyne.Toolkit.Results;
using Microsoft.Extensions.Options;

namespace Nexus.Bff.Infrastructure.Clients.UserManagement
{
    public partial class UserManagementService(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonOptions) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value;
        
        protected async Task<Result<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errors = await TryReadErrors(response);
                return Result<T>.Failure(errors ?? [new Error(ErrorCode.Server, $"HTTP {response.StatusCode}")]);
            }

            if (response.Content.Headers.ContentLength == 0)
                return Result<T>.Success(default!);

            var content = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            return Result<T>.Success(content!);
        }

        protected async Task<Result> HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errors = await TryReadErrors(response);
                return Result.Failure(errors ?? [new Error(ErrorCode.Server, $"HTTP {response.StatusCode}")]);
            }

            return Result.Success();
        }

        private async Task<Error[]?> TryReadErrors(HttpResponseMessage response)
        {
            try
            {
                if (response.Content.Headers.ContentLength == 0)
                    return null;

                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                    return null;

                return await response.Content.ReadFromJsonAsync<Error[]>(_jsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}