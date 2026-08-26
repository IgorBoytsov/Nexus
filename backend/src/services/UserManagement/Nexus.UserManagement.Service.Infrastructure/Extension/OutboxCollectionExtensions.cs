using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Abstractions.Outbox;
using Nexus.UserManagement.Service.Domain.Events;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class OutboxCollectionExtensions
    {
        public static IServiceCollection RegisterOutbox(this IServiceCollection services)
        {
            services.AddScoped<IDbContextOutbox, DbContextOutbox>();
            services.AddSingleton<IOutboxSignal, OutboxSignal>();
            services.AddHostedService<OutboxProcessor>();
            services.AddHostedService<OutboxCleanupService>();

            services.AddSingleton(_
                => new EventTypeMappingRegistry()
                    .Map<UserPasswordResetDomainEvent, UserPasswordResetIntegrationEvent>()
                    .Map<UserAccountDeletedDomainEvent, UserAccountDeletedIntegrationEvent>()
                    .Map<PasswordResetRequestedDomainEvent, PasswordResetRequestedIntegrationEvent>()
                    .Map<ChangeEmailRequestedDomainEvent, ChangeEmailRequestedIntegrationEvent>());

            return services;
        }
    }
}