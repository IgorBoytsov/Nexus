using Microsoft.Extensions.Logging;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Shared.Contracts.Messaging.Abstractions;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.Authentication.Service.Application.Features.EventHandlers
{
    public sealed class UserAccountDeletedIntegrationEventHandler(
        IAccessDataRepository repository, 
        ILogger<UserAccountDeletedIntegrationEventHandler> logger) : IIntegrationEventHandler<UserAccountDeletedIntegrationEvent>
    {
        public async Task HandleAsync(UserAccountDeletedIntegrationEvent @event, CancellationToken cancellationToken)
        {            
            logger.LogInformation("Получение событие удаление учетной записи для UserId: {UserId}", @event.UserId);

            var countClosedSessions = await repository.CloseSessions(@event.UserId, @event.OccurredOnUtc);

            logger.LogInformation("Обработка событие удаление учетной записи для UserId: {UserId} выполнена успешно. Было закрыто {countClosedSessions} активных сессий", @event.UserId, countClosedSessions);
        }
    }
}