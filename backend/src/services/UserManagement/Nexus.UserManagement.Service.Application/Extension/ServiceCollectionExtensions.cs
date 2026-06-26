using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Behaviors;
using Shared.Contracts.Security.Interfaces;
using Shared.Security.Hasher;
using Shared.Validations.Extensions;

namespace Nexus.UserManagement.Service.Application.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddValidations(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

            return services;
        }
    }
}