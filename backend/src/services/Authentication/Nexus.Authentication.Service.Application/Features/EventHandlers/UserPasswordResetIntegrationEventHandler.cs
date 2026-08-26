using Microsoft.Extensions.Logging;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Shared.Abstractions.Messaging.Abstractions;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.Authentication.Service.Application.Features.EventHandlers
{
    public sealed class UserPasswordResetIntegrationEventHandler(
        IAccessDataRepository repository, 
        ILogger<UserPasswordResetIntegrationEventHandler> logger) : IIntegrationEventHandler<UserPasswordResetIntegrationEvent>
    {
        public async Task HandleAsync(UserPasswordResetIntegrationEvent @event, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение событие сброса пароля для UserId: {UserId}", @event.UserId);

            var countClosedSessions = await repository.CloseSessions(@event.UserId, @event.OccurredOnUtc);

            logger.LogInformation("Обработка событие сброса пароля для UserId: {UserId} выполнена успешно. Было закрыто {countClosedSessions} активных сессий", @event.UserId, countClosedSessions);
        }
    }
}