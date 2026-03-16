using System.Reflection;
using Nexus.Bff.Infrastructure.Clients;

namespace Nexus.Bff.Extensions
{
 public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var authBaseUrl = configuration["Urls:AuthServicesBase"];
            string authenticationServices = "AuthenticationServices";
            services.AddHttpClient<IAuthClient, AuthClient>(authenticationServices, client => client.BaseAddress = new Uri(authBaseUrl!));

            var userManagementBaseUrl = configuration["Urls:UserManagementBase"];
            string userManagementService = "UserManagementService";
            services.AddHttpClient<IUserManagementService, UserManagementService>(userManagementService, client => client.BaseAddress = new Uri(userManagementBaseUrl!));

            return services;
        }
    }
}