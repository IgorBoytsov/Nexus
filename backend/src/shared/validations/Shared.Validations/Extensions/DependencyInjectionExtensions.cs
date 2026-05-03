using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validations.Common.Behaviors;

namespace Shared.Validations.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddValidations(this IServiceCollection services, Assembly assembly)
        {
            services.AddValidatorsFromAssembly(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}