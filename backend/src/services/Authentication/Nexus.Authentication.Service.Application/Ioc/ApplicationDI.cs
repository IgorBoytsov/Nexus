using Crossdyne.Security.Abstractions;
using Crossdyne.Security.Cryptography;
using Crossdyne.Security.Srp.Server;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Secure;
using Nexus.Authentication.Service.Application.Services;
using Shared.Security.Hasher;
using Shared.Security.Verifiers;
using Shared.Validations.Extensions;
using System.Reflection;

namespace Nexus.Authentication.Service.Application.Ioc
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidations(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IVerifierProtector, RsaDecryptor>();
            services.AddTransient<ISrpServer, SrpServerService>();
            services.AddSingleton<ICryptoServices, CryptoService>();

            return services;
        }
    }
}