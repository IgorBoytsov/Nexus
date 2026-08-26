using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Validations.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddValidations(this IServiceCollection services, Assembly assembly)
        {
            List<Assembly> assemblies = [assembly, Assembly.GetExecutingAssembly()];

            services.AddValidatorsFromAssemblies(assemblies);
            
            return services;
        }
    }
}