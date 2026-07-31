using Shared.Contracts.Messaging.Interfaces;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class EventTypeMappingRegistry
    {
        private readonly Dictionary<Type, Type> _mappings = new();

        public EventTypeMappingRegistry Map<TDomainEvent, TIntegrationEvent>()
            where TDomainEvent : IDomainEvent
            where TIntegrationEvent : IIntegrationEvent
        {
            _mappings[typeof(TDomainEvent)] = typeof(TIntegrationEvent);
            return this;
        }

        public Type GetIntegrationType(Type domainEventType)
        {
            if (_mappings.TryGetValue(domainEventType, out var integrationType))
                return integrationType;

            throw new InvalidOperationException($"Не найден маппинг для доменного события {domainEventType.Name} в интеграционное");
        }
    }
}