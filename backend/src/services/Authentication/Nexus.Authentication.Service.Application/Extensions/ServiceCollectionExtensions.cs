using System.Reflection;
using Crossdyne.Security.Abstractions;
using Crossdyne.Security.Cryptography;
using Crossdyne.Security.Srp.Server;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Secure;
using Nexus.Authentication.Service.Application.Services;
using Shared.Contracts.Security.Interfaces;
using Shared.Security.Hasher;
using Shared.Validations.Extensions;
using StackExchange.Redis;

namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidations(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IDataProtector, RsaDecryptor>();
            services.AddTransient<ISrpServer, SrpServerService>();
            services.AddSingleton<ICryptoServices, CryptoService>();

            services.AddSingleton(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisDistributedSynchronizationProvider(multiplexer.GetDatabase());
            });

            return services;
        }
    }
}