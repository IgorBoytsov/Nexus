using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexus.UserManagement.Service.Application.Extension
{
    public static class MappingCollectionExtensions
    {
        public static IServiceCollection RegisterMapping(this IServiceCollection services, IConfiguration configuration)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:AutoMapperKey"], currentAssembly);

            return services;
        }
    }
}