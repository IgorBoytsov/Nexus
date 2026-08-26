using Nexus.Bff.Extensions;
using Shared.Abstractions.Cache.Abstractions;
using Shared.Abstractions.Messaging.Abstractions;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.Bff.Features.Auth.EventHandlers
{
    public sealed class UserAccountDeletedIntegrationEventHandler(
        ICacheService cache, 
        ILogger<UserAccountDeletedIntegrationEventHandler> logger) : IIntegrationEventHandler<UserAccountDeletedIntegrationEvent>
    {
        public async Task HandleAsync(UserAccountDeletedIntegrationEvent @event, CancellationToken cancellationToken)
        {            
            logger.LogInformation("Получение событие удаление учетной записи для UserId: {UserId}", @event.UserId);

            var userSessionKey = RedisKeyExtensions.UserSessionsKey(@event.UserId.ToString());

            var sessionIds = await cache.SetMembersAsync(userSessionKey);

            int countClosedSessions = 0;

            if(sessionIds.Length > 0)
            {
                var keysToDelete = sessionIds
                    .Select(id => RedisKeyExtensions.SessionKey(id))
                    .Append(userSessionKey)
                    .ToArray();

                await cache.RemoveAsync(keysToDelete);

                countClosedSessions = sessionIds.Length;
            }
            else
            {
                await cache.RemoveAsync(userSessionKey);
            }

            logger.LogInformation("Обработка событие удаление учетной записи для UserId: {UserId} выполнена успешно. Было закрыто {countClosedSessions} активных сессий", @event.UserId, countClosedSessions);
        }
    }
}