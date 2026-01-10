using Microsoft.AspNetCore.SignalR;
using real_time_notification.Api.Hubs;
using real_time_notification.Application.Interfaces;

namespace real_time_notification.Application.Services;

public class NotificationService(ILogger<NotificationService> logger, IHubContext<ConnectionHub> hubContext)
    : INotificationService
{
    private readonly ILogger<NotificationService> _logger = logger;
    private readonly IHubContext<ConnectionHub> _hubContext = hubContext;

    public async Task SendNotificationAsync(string message, string userId, string title)
    {
        
        try
        {
            var targetedGroup = userId.Trim();
            _logger.LogInformation("Sending notification to {UserId}", targetedGroup);
            await _hubContext.Clients.Group(targetedGroup).SendAsync("ReceiveMessage", new
            {
                Title = title,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            /*await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Teste Geral");*/
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error in SendNotificationAsync:", e);
            throw;
        }
    }
}