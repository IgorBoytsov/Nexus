using Nexus.Bff.Handlers;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Infrastructure.Clients.UserManagement;

namespace Nexus.Bff.Extensions
{
    public static class HttpClientsServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<AccessTokenHandler>();

            services.AddHttpClient<IAuthClient, AuthClient>(client => client.BaseAddress = new Uri(configuration["Urls:AuthServicesBase"]!)).AddHttpMessageHandler<AccessTokenHandler>();
            services.AddHttpClient<IUserManagementService, UserManagementService>(client => client.BaseAddress = new Uri(configuration["Urls:UserManagementBase"]!)).AddHttpMessageHandler<AccessTokenHandler>();
            services.AddHttpClient<IFileService, FileService>(client => client.BaseAddress = new Uri(configuration["Urls:FileService"]!));

            return services;
        }
    }
}