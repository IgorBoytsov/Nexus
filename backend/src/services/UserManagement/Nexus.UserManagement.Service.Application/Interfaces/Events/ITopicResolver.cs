using Shared.Contracts.Messaging.Abstractions;

namespace Nexus.UserManagement.Service.Application.Interfaces.Events
{
    public interface ITopicResolver
    {
        public ITopicResolver Map<TEvent>(string topic) where TEvent : IIntegrationEvent;
        string Resolve(string eventType);
    }
}