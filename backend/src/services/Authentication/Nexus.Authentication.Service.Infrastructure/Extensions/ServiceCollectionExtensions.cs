using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Authentication.Service.Application.Features.EventHandlers;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Infrastructure.BackgroundServices;
using Nexus.Authentication.Service.Infrastructure.HttpClients;
using Nexus.Authentication.Service.Infrastructure.Persistence;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas;
using Shared.Contracts.Messaging.Interfaces;
using Shared.Contracts.UserManagement.Events;
using Shared.Redis;

namespace Nexus.Authentication.Service.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.Configure<ConsumerConfig>(configuration.GetSection("Kafka:Consumer"));

            services.AddDbContext<AuthenticationContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddCashService(configuration);
            services.AddHttpClient<IUserManagementServiceClient, UserManagementServiceClient>(client => client.BaseAddress = new Uri(configuration["ServiceUrls:UserManagement"]!));
            services.AddScoped<IAccessDataRepository, AccessDataRepository>();

            services.AddHostedService<TokenCleanupBackgroundService>();

            services.AddScoped<IIntegrationEventHandler<UserPasswordResetIntegrationEvent>, UserPasswordResetIntegrationEventHandler>();
            services.AddKafkaConsumer<UserPasswordResetIntegrationEvent>("user-management.user.password-reset");

            services.AddScoped<IIntegrationEventHandler<UserAccountDeletedIntegrationEvent>, UserAccountDeletedIntegrationEventHandler>();
            services.AddKafkaConsumer<UserAccountDeletedIntegrationEvent>("user-management.user.account-delete");

            return services;
        }
    }
}