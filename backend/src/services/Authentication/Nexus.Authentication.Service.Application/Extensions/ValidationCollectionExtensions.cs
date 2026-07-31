using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validations.Extensions;

namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class ValidationCollectionExtensions
    {
        public static IServiceCollection RegisterValidation(this IServiceCollection services)
        {
            services.AddValidations(Assembly.GetEntryAssembly()!);

            return services;
        }
    }
}