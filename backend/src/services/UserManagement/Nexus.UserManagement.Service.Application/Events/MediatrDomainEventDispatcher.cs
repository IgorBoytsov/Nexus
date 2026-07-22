using MediatR;
using Microsoft.Extensions.Logging;
using Nexus.UserManagement.Service.Application.Interfaces.Events;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Application.Events
{
    public class MediatRDomainEventDispatcher(
        IPublisher publisher, 
        ILogger<MediatRDomainEventDispatcher> logger) : IDomainEventDispatcher
    {
        public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken ct = default)
        {
            if (events.Count == 0)
                return;

            var tasks = events.Select(async domainEvent =>
            {
                try
                {
                    var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());

                    var notification = Activator.CreateInstance(wrapperType, domainEvent) 
                        ?? throw new InvalidOperationException($"Не удалось создать обёртку для {domainEvent.GetType().Name}");

                    await publisher.Publish(notification, ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "In-process обработчик упал для события {EventType}. Основной flow не прерван.", domainEvent.GetType().Name);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}