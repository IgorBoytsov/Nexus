using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Extensions.Dapper;
using Npgsql;
using Shared.Dapper.TypeHandlers;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class DapperCollectionExtensions
    {
        public static IServiceCollection RegisterReadonlyDatabase(this IServiceCollection services, string dateBaseConnectionString)
        {
            services.AddSingleton<IDbConnection>(sp => new NpgsqlConnection(dateBaseConnectionString));
            
            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new JsonListStringHandler());
            SqlMapper.AddTypeHandler(new S3KeyResponseTypeHandler());

            return services;
        }
    }
}