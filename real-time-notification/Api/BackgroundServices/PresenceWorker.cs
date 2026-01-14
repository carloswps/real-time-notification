using Microsoft.EntityFrameworkCore;
using real_time_notification.Infra;

namespace real_time_notification.Api.BackgroundServices;

public class PresenceWorker(IServiceProvider serviceProvider, ILogger<PresenceWorker> logger) : BackgroundService
{
    private readonly ILogger<PresenceWorker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Presence worker service is started.");
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var timeoutThreshold = DateTime.UtcNow.AddMinutes(-2);

                    var zombies = await context.Users
                        .Where(u => u.IsOnline && u.LastOnline < timeoutThreshold)
                        .ToListAsync(stoppingToken);

                    if (zombies.Any())
                    {
                        _logger.LogInformation("Updating {zombieCount} zombie users status to offline.", zombies.Count);

                        foreach (var zombie in zombies) zombie.IsOnline = false;

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Zombie users status updated.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in presence worker service.");
            }

        await Task.Delay(_checkInterval, stoppingToken);
    }
}