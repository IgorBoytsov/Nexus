using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Infrastructure.HttpClients;
using Nexus.Authentication.Service.Infrastructure.Persistence;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas;
using Shared.Redis;

namespace Nexus.Authentication.Service.Infrastructure.Ioc
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AuthenticationContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddCashService(configuration);
            services.AddHttpClient<IUserManagementServiceClient, UserManagementServiceClient>(client => client.BaseAddress = new Uri(configuration["ServiceUrls:UserManagement"]!));
            services.AddScoped<IAccessDataRepository, AccessDataRepository>();
            
            return services;
        }
    }
}