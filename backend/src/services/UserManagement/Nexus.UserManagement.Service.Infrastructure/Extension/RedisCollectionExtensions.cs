using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Redis;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class RedisCollectionExtensions
    {
        public static IServiceCollection RegisterCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCashService(configuration);

            return services;
        }
    }
}