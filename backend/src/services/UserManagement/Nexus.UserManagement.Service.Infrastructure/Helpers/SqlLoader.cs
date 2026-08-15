using System.Reflection;
using System.Text;

namespace Nexus.UserManagement.Service.Infrastructure.Helpers
{
    public static class SqlLoader
    {
        private const string BaseNamespace = "Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories";
        private const string DefaultFolderName = "Sql";

        public static string Load(string repositoryFolderName, string filename, string? folderNameForQueries = null)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var sb = new StringBuilder();

            sb.Append($"{BaseNamespace}.{repositoryFolderName}");

            if (string.IsNullOrWhiteSpace(folderNameForQueries))
                sb.Append($".{DefaultFolderName}.{filename}.sql");
            else
                sb.Append($".{folderNameForQueries}.{filename}.sql");

            var resourceName = sb.ToString();

            using var stream = assembly!.GetManifestResourceStream(resourceName) ??
                 throw new InvalidOperationException($"SQL файл '{resourceName}' не найден.");

            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd().Trim();
        }
    }
}