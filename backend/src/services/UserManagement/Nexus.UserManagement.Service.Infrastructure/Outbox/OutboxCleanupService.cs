using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class OutboxCleanupService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<OutboxCleanupService> logger) : BackgroundService
    {
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(configuration.GetValue<double>("OutboxCleanup:IntervalHours", 24));
        private static readonly TimeSpan _successfulRetention = TimeSpan.FromDays(14);
        private static readonly TimeSpan _failedRetention = TimeSpan.FromDays(90);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("OutboxCleanupService запущен. Интервал выполнения: {interval}", _cleanupInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при очистке Outbox таблицы");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private async Task CleanupAsync(CancellationToken stoppingToken)
        {
           using var scope = scopeFactory.CreateScope();
           var context = scope.ServiceProvider.GetRequiredService<UserManagementContext>();

            var successfulThreshold = DateTime.UtcNow - _successfulRetention;
            var failedThreshold = DateTime.UtcNow - _failedRetention;

            var deletedSuccess = await context.Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc != null && m.ProcessedOnUtc < successfulThreshold && m.Error != null)
                .ExecuteDeleteAsync(stoppingToken);

            var deletedFailed = await context.Set<OutboxMessage>()
                .Where(m =>  m.ProcessedOnUtc != null && m.ProcessedOnUtc < failedThreshold && m.Error != null)
                .ExecuteDeleteAsync(stoppingToken);

            logger.LogInformation("Outbox очистка завершена. Удалено успешных: {Success}, упавших: {Failed}", deletedSuccess, deletedFailed);
        }
    }
}