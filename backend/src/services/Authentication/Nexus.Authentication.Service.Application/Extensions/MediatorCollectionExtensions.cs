using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Behaviors;

namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class MediatorCollectionExtensions
    {
        public static IServiceCollection RegisterMediator(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}