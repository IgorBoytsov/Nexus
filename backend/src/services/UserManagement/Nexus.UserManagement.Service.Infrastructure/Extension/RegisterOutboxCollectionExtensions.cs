using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Interfaces.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Outbox;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class RegisterOutboxCollectionExtensions
    {
        public static IServiceCollection RegisterWriteDatabase(this IServiceCollection services, IConfiguration configuration, string dateBaseConnectionString)
        {
            services.AddScoped<IDbContextOutbox, DbContextOutbox>();
            services.AddSingleton<IOutboxSignal, OutboxSignal>();
            services.AddHostedService<OutboxProcessor>();
            services.AddHostedService<OutboxCleanupService>();

            return services;
        }
    }
}