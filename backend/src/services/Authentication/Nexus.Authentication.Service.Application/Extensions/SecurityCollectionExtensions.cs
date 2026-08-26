using Crossdyne.Security.Cryptography;
using Crossdyne.Security.Srp.Server;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Secure;
using Nexus.Authentication.Service.Application.Services;
using Shared.Abstractions.Security;
using StackExchange.Redis;

namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class SecurityCollectionExtensions
    {
        public static IServiceCollection RegisterSecurity(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IDataProtector, RsaDecryptor>();
            services.AddCrossdyneCryptography();
            services.AddCrossdyneSrpServer();

            services.AddSingleton<IDistributedLockProvider>(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisDistributedSynchronizationProvider(multiplexer.GetDatabase());
            });

            return services;
        }
    }
}