using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Services.Hasher;
using System.Reflection;

namespace Nexus.UserManagement.Service.Application.Ioc
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

            return services;
        }
    }
}