using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Infrastructure.HttpClients;

namespace Nexus.Authentication.Service.Infrastructure.Extensions
{
    public static class HttpClientCollectionExtensions
    {
        public static IServiceCollection RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IUserManagementServiceClient, UserManagementServiceClient>(client => client.BaseAddress = new Uri(configuration["ServiceUrls:UserManagement"]!));
            
            return services;
        }
    }
}