using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Abstractions.Clients;
using Nexus.UserManagement.Service.Infrastructure.Clients;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class HttpClientsCollectionExtensions
    {
        public static IServiceCollection RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            string? fileServiceUrl = configuration["Urls:FileService"];
            services.AddHttpClient<IFileService, FileService>(client => client.BaseAddress = new Uri(fileServiceUrl!));

            return services;
        }
    }
}