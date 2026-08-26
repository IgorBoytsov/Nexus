using System.Text.RegularExpressions;
using Nexus.UserManagement.Service.Application.Interfaces.Events;
using Shared.Abstractions.Messaging.Abstractions;

namespace Nexus.UserManagement.Service.Infrastructure.MessageBroker
{
    public sealed class TopicResolver : ITopicResolver
    {
        private readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase);

        public ITopicResolver Map<TEvent>(string topic) where TEvent : IIntegrationEvent
        {
            var key = typeof(TEvent).FullName ?? throw new InvalidOperationException($"Тип {typeof(TEvent).Name} не имеет FullName");

            _map[key] = topic;
            return this;
        }

        public string Resolve(string eventType)
        {
            if (_map.TryGetValue(eventType, out var topic))
                return topic;

            var shortName = eventType
                .Split('.')
                .Last()
                .Replace("DomainEvent", "")
                .Replace("IntegrationEvent", "")
                .Replace("Event", "");

            return $"user-management.{ToKebabCase(shortName)}";
        }

        private static string ToKebabCase(string input) => Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}