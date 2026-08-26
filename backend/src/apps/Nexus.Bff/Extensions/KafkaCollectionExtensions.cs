using Confluent.Kafka;
using Nexus.Bff.Features.Auth.EventHandlers;
using Shared.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Events;
using Shared.Messaging;

namespace Nexus.Bff.Extensions
{
    public static class KafkaCollectionExtensions
    {
        public static IServiceCollection RegisterMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsumerConfig>(configuration.GetSection("Kafka:Consumer"));

            services.AddScoped<IIntegrationEventHandler<UserPasswordResetIntegrationEvent>, UserPasswordResetIntegrationEventHandler>();
            services.AddKafkaConsumer<UserPasswordResetIntegrationEvent>("user-management.user.password-reset");

            services.AddScoped<IIntegrationEventHandler<UserAccountDeletedIntegrationEvent>, UserAccountDeletedIntegrationEventHandler>();
            services.AddKafkaConsumer<UserAccountDeletedIntegrationEvent>("user-management.user.account-delete");

            return services;
        }
    }
}