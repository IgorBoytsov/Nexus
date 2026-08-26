using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Behaviors;
using Shared.Application.Behaviors;

namespace Nexus.UserManagement.Service.Application.Extension
{
    public static class MediatorCollectionExtensions
    {
        public static IServiceCollection RegisterMediator(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(currentAssembly));

            return services;
        }
    }
}