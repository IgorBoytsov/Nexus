using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Extensions
{
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// Создает View из вложенного SQL-ресурса.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="viewName">Имя создаваемой View (например, "V_Genders")</param>
        public static void CreateView(this MigrationBuilder migrationBuilder, string viewName)
        {
            var resourcePrefix = "Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Sql.Views";
            var resourceName = $"{resourcePrefix}.{viewName}.sql";

            var sqlScript = GetSqlFromResource(resourceName);
            migrationBuilder.Sql(sqlScript);
        }

        /// <summary>
        /// Удаляет View.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="viewName">Имя удаляемой View (например, "V_Genders")</param>
        public static void DropView(this MigrationBuilder migrationBuilder, string viewName)
        {
            migrationBuilder.Sql($@"DROP VIEW IF EXISTS ""{viewName}"";");
        }

        private static string GetSqlFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Не удалось найти встроенный ресурс: {resourceName}");

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
