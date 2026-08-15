using System.Text.Json;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.FileService;
using Shared.Kernel.Errors;

namespace Nexus.Bff.Infrastructure.Clients
{
    internal sealed class FileService(HttpClient client) : IFileService 
    {
        private readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true 
        };

        public async Task<Result<string>> GetUrl(string bucket, string folder, string key)
        {
            var response = await client.GetAsync($"api/files/url?bucket={bucket}&folder={folder}&key={key}");
            
            if (!response.IsSuccessStatusCode)
                return Result<string>.Failure(new Error(AppErrors.Api, await response.Content.ReadAsStringAsync()));

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UrlResponse>(content, options);
            
            return Result<string>.Success(result!.Url);
        }
    }
}