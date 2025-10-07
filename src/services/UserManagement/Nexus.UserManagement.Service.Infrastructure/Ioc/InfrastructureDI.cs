using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Infrastructure.Ioc
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<WriteContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IWriteDbContext>(provider => provider.GetRequiredService<WriteContext>());

            services.AddDbContext<ReadContext>(option =>
            {
                option.UseNpgsql(connectionString);
                option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<ReadContext>());

            return services;
        }
    }
}