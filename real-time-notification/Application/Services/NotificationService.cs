using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using real_time_notification.Api.Hubs;
using real_time_notification.Application.DTO;
using real_time_notification.Application.Interfaces;
using real_time_notification.Infra;

namespace real_time_notification.Application.Services;

public class NotificationService(
    ILogger<NotificationService> logger,
    IHubContext<ConnectionHub> hubContext,
    AppDbContext context)
    : INotificationService
{
    private readonly ILogger<NotificationService> _logger = logger;
    private readonly IHubContext<ConnectionHub> _hubContext = hubContext;
    private readonly AppDbContext _context = context;

    public async Task SendNotificationAsync(string message, string userId, string title)
    {
        
        try
        {
            var targetedGroup = userId.Trim();
            _logger.LogInformation("Sending notification to {UserId}", targetedGroup);
            await _hubContext.Clients.Group(targetedGroup).SendAsync("ReceiveMessage", new
            {
                Title = title,
                Message = message
            });

        }
        catch (Exception e)
        {
            _logger.LogError("Error in SendNotificationAsync:", e);
            throw;
        }
    }

    public async Task<UserStatusDto> GetUserStatus(int userId)
    {
        try
        {
            var userStatus = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserStatusDto { IsOnline = u.IsOnline, LastOnline = u.LastOnline, Id = u.Id })
                .FirstOrDefaultAsync();

            return userStatus;
        }
        catch (Exception e)
        {
            _logger.LogError("Erro in GetUserStatus:", e);
            throw;
        }
    }
}