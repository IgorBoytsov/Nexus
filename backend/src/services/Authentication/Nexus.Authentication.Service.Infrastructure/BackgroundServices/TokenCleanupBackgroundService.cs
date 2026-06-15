using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.Authentication.Service.Infrastructure.BackgroundServices
{
    public sealed class TokenCleanupBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IConfiguration configuration) : BackgroundService
    {
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(configuration.GetValue<double>("TokenCleanup:IntervalHours", 1));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Сервис очистки токенов запущен. Интервал выполнения: {_cleanupInterval}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();

                    int deletedCount = await dbContext.AccessData.Where(x => x.ExpiryDate < DateTime.UtcNow).ExecuteDeleteAsync(stoppingToken);

                    if (deletedCount > 0)
                    {
                        Console.WriteLine($"Удалено просроченных токенов: {deletedCount}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при очистке просроченных токенов: {ex}");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            Console.WriteLine("Сервис очистки токенов остановлен.");
        }
    }
}