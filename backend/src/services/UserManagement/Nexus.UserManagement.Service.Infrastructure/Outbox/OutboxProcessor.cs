using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.UserManagement.Service.Application.Interfaces.Events;
using Nexus.UserManagement.Service.Application.Interfaces.Outbox;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.Abstractions.Messaging.Abstractions;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IOutboxSignal outboxSignal,
        ILogger<OutboxProcessor> logger) : BackgroundService
    {
        private static readonly TimeSpan FallbackPollingInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan RetryBaseDelay = TimeSpan.FromSeconds(5);
        private const int BatchSize = 100;
        private const int MaxRetries = 5;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("OutboxProcessor запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                await WaitForSignalOrTimeout(stoppingToken);
                await ProcessPendingMessages(stoppingToken);
            }
        }

        private async Task WaitForSignalOrTimeout(CancellationToken ct)
        {
            try
            {
                var signalTask = outboxSignal.Reader.WaitToReadAsync(ct).AsTask();
                var timeoutTask = Task.Delay(FallbackPollingInterval, ct);
                await Task.WhenAny(signalTask, timeoutTask);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                
            }
        }

        private async Task ProcessPendingMessages(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserManagementContext>();
            var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
            var topicResolver = scope.ServiceProvider.GetRequiredService<ITopicResolver>();

            var messages = await context.Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null && m.NextRetryAt <= DateTime.UtcNow)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(BatchSize)
                .ToListAsync(stoppingToken);

            if (messages.Count == 0)
                return;

            logger.LogDebug("Обрабатывается {Count} outbox сообщений", messages.Count);

            int processed = 0, failed = 0;

            foreach (var message in messages)
            {
                try
                {
                    var topic = topicResolver.Resolve(message.EventType);
                    await eventPublisher.PublishAsync(topic, message.Content);

                    // var eventObject = JsonSerializer.Deserialize<object>(message.Content);
                    // await eventPublisher.PublishAsync(topic, eventObject);

                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;
                    processed++;

                    logger.LogDebug("Публикация outbox сообщения {id} в топик {Topic}", message.Id, topic);
                }
                catch (Exception ex)
                {
                    failed++;
                    message.RetryCount++;
                    message.Error = $"{ex.GetType().Name}: {ex.Message}";

                    if (message.RetryCount >= MaxRetries)
                    {
                        logger.LogError(ex,"Сообщение {Id} превысило лимит попыток ({MaxRetries}). Требуется ручная обработка/DLQ.", message.Id, MaxRetries);
                        message.ProcessedOnUtc = DateTime.UtcNow;
                    }
                    else
                    {
                        var delay = RetryBaseDelay * Math.Pow(2, message.RetryCount - 1);
                        message.NextRetryAt = DateTime.UtcNow.Add(delay);

                        logger.LogWarning(ex, "Ошибка публикации {Id}. Попытка {RetryCount}/{MaxRetries}. Следующая попытка: {NextRetryAt}", message.Id, message.RetryCount, MaxRetries, message.NextRetryAt);
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }

            if (processed > 0)
                await context.SaveChangesAsync(stoppingToken);

            logger.LogDebug("Batch завершён. Успешно: {Processed}, Ошибок: {Failed}", processed, failed);
        }
    }
}