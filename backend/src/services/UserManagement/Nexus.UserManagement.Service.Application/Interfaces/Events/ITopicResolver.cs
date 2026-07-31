using Shared.Contracts.Messaging.Interfaces;

namespace Nexus.UserManagement.Service.Application.Interfaces.Events
{
    public interface ITopicResolver
    {
        public ITopicResolver Map<TEvent>(string topic) where TEvent : IIntegrationEvent;
        string Resolve(string eventType);
    }
}