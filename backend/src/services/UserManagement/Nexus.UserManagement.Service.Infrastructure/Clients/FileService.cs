using System.Net.Http.Headers;
using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Clients;
using Shared.Kernel.Errors;

namespace Nexus.UserManagement.Service.Infrastructure.Clients
{
    internal sealed class FileService(HttpClient client) : IFileService 
    {
        public async Task<Result<Unit>> Upload(string bucket, string folderPath, string fileName, string mimeType, Stream file)
        {
            if (file.CanSeek)
                file.Position = 0;
        
            using var formData = new MultipartFormDataContent
            {
                { new StringContent(bucket), "bucket" },
                { new StringContent(folderPath), "folder" },
                { new StringContent(fileName), "key" },
            };

            var streamContent = new StreamContent(file);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            formData.Add(streamContent, "file", fileName);

            var response = await client.PostAsync("api/files/upload", formData);

            if (!response.IsSuccessStatusCode)
                return new Error(AppErrors.Api, await response.Content.ReadAsStringAsync());

            return Result.Success();
        }

        public async Task<Result> Delete(string bucket, string folder, string key)
        {
            var response = await client.DeleteAsync($"api/files?bucket={bucket}&folder={folder}&key={key}");

            if (!response.IsSuccessStatusCode)
                return Result.Failure(new Error(AppErrors.Api, await response.Content.ReadAsStringAsync()));

            return Result.Success();
        }
    }
}