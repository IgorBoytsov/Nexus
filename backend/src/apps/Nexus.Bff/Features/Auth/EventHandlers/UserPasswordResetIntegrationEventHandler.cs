using Nexus.Bff.Extensions;
using Shared.Abstractions.Cache.Abstractions;
using Shared.Contracts.Messaging.Abstractions;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.Bff.Features.Auth.EventHandlers
{
    public sealed class UserPasswordResetIntegrationEventHandler(
        ICacheService cache, 
        ILogger<UserPasswordResetIntegrationEventHandler> logger) : IIntegrationEventHandler<UserPasswordResetIntegrationEvent>
    {
        public async Task HandleAsync(UserPasswordResetIntegrationEvent @event, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение событие сброса пароля для UserId: {UserId}", @event.UserId);

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

            logger.LogInformation("Обработка событие сброса пароля для UserId: {UserId} выполнена успешно. Было закрыто {countClosedSessions} активных сессий", @event.UserId, countClosedSessions);
        }
    }
}