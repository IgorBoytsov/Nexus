using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Infrastructure.BackgroundServices;

namespace Nexus.Authentication.Service.Infrastructure.Extensions
{
    public static class BackgroundServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<TokenCleanupBackgroundService>();

            return services;
        }
    }
}