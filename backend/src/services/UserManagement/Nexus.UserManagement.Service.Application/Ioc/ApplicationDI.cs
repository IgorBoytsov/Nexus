using Microsoft.Extensions.DependencyInjection;
using Shared.Security.Hasher;
using Shared.Validations.Extensions;
using System.Reflection;

namespace Nexus.UserManagement.Service.Application.Ioc
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidations(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

            return services;
        }
    }
}