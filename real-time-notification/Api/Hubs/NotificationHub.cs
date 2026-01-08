using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using real_time_notification.Domain.Entities;

namespace real_time_notification.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    private string? UserId => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            await base.OnConnectedAsync();
            return;
        }

        _logger.LogInformation("User {UserId} connected with ConnectionId {ConnectionId}", UserId,
            Context.ConnectionId);

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);
            _logger.LogInformation("User {UserId} added successfully to the group.", UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in added User to the group {UserId}", UserId);
            throw;
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (string.IsNullOrEmpty(UserId))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        if (exception != null)
            _logger.LogWarning(exception, "User {UserId} disconnected with error.", UserId);
        else
            _logger.LogWarning(exception, "User {UserId} disconnected.", UserId);

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);
            _logger.LogInformation("User {UserId} removed the group.", UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro removing user {UserId} from group.", UserId);
            throw;
        }

        await base.OnDisconnectedAsync(exception);
    }
}