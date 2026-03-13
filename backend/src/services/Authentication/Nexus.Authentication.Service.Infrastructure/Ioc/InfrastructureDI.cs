using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nexus.Authentication.Service.Application;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Nexus.Authentication.Service.Infrastructure.Redis;
using StackExchange.Redis;

namespace Nexus.Authentication.Service.Infrastructure.Ioc
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AuthenticationContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AuthenticationContext>());

            services.Configure<RedisOptions>(option => configuration.GetSection(RedisOptions.SectionName));
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionRedisString = configuration["Redis:ConnectionString"];
                
                var config = ConfigurationOptions.Parse(connectionRedisString!);

                config.AbortOnConnectFail = false;
                config.ConnectRetry = 3;
                config.ConnectTimeout = 5000;

                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}