using System.Text.Json;
using AutoMapper;
using Nexus.UserManagement.Service.Application.Interfaces.Outbox;
using Nexus.UserManagement.Service.Domain.Events;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.Contracts.UserManagement.Events;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class DbContextOutbox(
        UserManagementContext context, 
        IMapper mapper) : IDbContextOutbox
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        private static readonly Dictionary<Type, Type> EventMapping = new()
        {
            [typeof(UserPasswordResetDomainEvent)] = typeof(UserPasswordResetIntegrationEvent),
            [typeof(UserAccountDeletedDomainEvent)] = typeof(UserAccountDeletedIntegrationEvent),
            [typeof(PasswordResetRequestedDomainEvent)] = typeof(PasswordResetRequestedIntegrationEvent),
        };

        public void Append(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            if (domainEvents.Count == 0)
                return;

            var outboxMessages = domainEvents.Select(e =>
            {
                var sourceType = e.GetType();

                if (!EventMapping.TryGetValue(sourceType, out var destType))
                    throw new InvalidOperationException($"Не найден маппинг для доменного события {sourceType.Name} в интеграционное");

                var integrationEvent = mapper.Map(e, sourceType, destType);

                var eventType = integrationEvent.GetType();
                var typeName = eventType.FullName ?? throw new InvalidOperationException($"Тип {eventType.Name} не имеет FullName");

                string payload;
                try
                {
                    payload = JsonSerializer.Serialize(integrationEvent, eventType, JsonOptions);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Сериализация события {typeName} не удалась", ex);
                }

                return new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    EventType = typeName,
                    Content = payload,
                    OccurredOnUtc = e.OccurredOnUtc,
                    ProcessedOnUtc = null,
                    RetryCount = 0,
                    Error = null,
                    NextRetryAt = DateTime.UtcNow
                };
            });

            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }
    }
}