using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas;

namespace Nexus.Authentication.Service.Infrastructure.Extensions
{
    public static class RepositoryCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccessDataRepository, AccessDataRepository>();

            return services;
        }
    }
}