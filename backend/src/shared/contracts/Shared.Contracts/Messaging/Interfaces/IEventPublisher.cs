namespace Shared.Contracts.Messaging.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string topic, TEvent @event, string? key = null);
    }
}