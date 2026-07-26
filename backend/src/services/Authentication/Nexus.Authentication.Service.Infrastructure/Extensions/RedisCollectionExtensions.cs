using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Redis;

namespace Nexus.Authentication.Service.Infrastructure.Extensions
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