using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validations.Extensions;

namespace Nexus.UserManagement.Service.Application.Extension
{
    public static class ValidationCollectionExtensions
    {
        public static IServiceCollection RegisterValidation(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            services.AddValidations(currentAssembly!);

            return services;
        }
    }
}