using System.Reflection;
using MediatR;
using Nexus.Bff.Services;
using Shared.Application.Behaviors;
using Shared.Redis;

namespace Nexus.Bff.Extensions
{
 public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            
            services.AddSingleton<IJwtReadService, JwtReadService>();
            services.AddCashService(configuration);
            
            return services;
        }
    }
}