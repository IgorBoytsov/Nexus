using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Interfaces.Clients
{
    public interface IFileService
    {
        Task<Result<Unit>> Upload(string bucket, string folderPath, string fileName, string mimeType, Stream file);
        Task<Result> Delete(string bucket, string folder, string key);
    }
}