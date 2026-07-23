using System.Text.RegularExpressions;
using Nexus.UserManagement.Service.Application.Interfaces.Events;

namespace Nexus.UserManagement.Service.Infrastructure.MessageBroker
{
    public sealed class TopicResolver : ITopicResolver
    {
        private readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Shared.Contracts.UserManagement.Events.UserPasswordResetIntegrationEvent"] = "user-management.user.password-reset",
            ["Shared.Contracts.UserManagement.Events.UserAccountDeletedIntegrationEvent"] = "user-management.user.account-delete"
        };

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