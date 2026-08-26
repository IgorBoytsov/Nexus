using System.Text.Json;
using AutoMapper;
using Nexus.UserManagement.Service.Application.Abstractions.Outbox;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class DbContextOutbox(
        UserManagementContext context, 
        IMapper mapper,
        EventTypeMappingRegistry mappingRegistry) : IDbContextOutbox
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public void Append(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            if (domainEvents.Count == 0)
                return;

            var outboxMessages = domainEvents.Select(e =>
            {
                var sourceType = e.GetType();
                var destType = mappingRegistry.GetIntegrationType(sourceType);

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