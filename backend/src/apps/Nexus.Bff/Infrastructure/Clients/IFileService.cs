using Crossdyne.Toolkit.Results;

namespace Nexus.Bff.Infrastructure.Clients
{
    public interface IFileService
    {
        Task<Result<string>> GetUrl(string bucket, string folder, string key);
    }
}