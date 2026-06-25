using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validations.Behaviors;

namespace Shared.Validations.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddValidations(this IServiceCollection services, Assembly assembly)
        {
            List<Assembly> assemblies = [assembly, Assembly.GetExecutingAssembly()];

            services.AddValidatorsFromAssemblies(assemblies);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}