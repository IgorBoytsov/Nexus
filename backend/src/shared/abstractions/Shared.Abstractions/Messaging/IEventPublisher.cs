namespace Shared.Abstractions.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string topic, TEvent @event, string? key = null);
    }
}