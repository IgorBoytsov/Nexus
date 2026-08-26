namespace Shared.Abstractions.Messaging.Abstractions
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string topic, TEvent @event, string? key = null);
    }
}