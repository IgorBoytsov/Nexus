using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.Authentication.Service.Infrastructure.Ioc
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AuthenticationContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AuthenticationContext>());

            return services;
        }
    }
}