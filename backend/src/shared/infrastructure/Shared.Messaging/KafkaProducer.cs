using System.Text.Json;
using Confluent.Kafka;
using Shared.Abstractions.Messaging.Abstractions;

namespace Shared.Messaging
{
    public sealed class KafkaProducer(IProducer<string, string> producer) : IEventPublisher
    {
        public async Task PublishAsync<TEvent>(string topic, TEvent @event, string? key = null)
        {
            var messageValue = @event is string stringEvent 
                ? stringEvent 
                : JsonSerializer.Serialize(@event);

            var message = new Message<string, string>
            {
                Key = key!,
                Value = messageValue
            };

            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, message);
            }
            catch (ProduceException<string, string>)
            {
                throw;
            }
        }
    }
}