using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using real_time_notification.Application.Interfaces;
using real_time_notification.Domain.Entities;

namespace real_time_notification.Api.Hubs;

[Authorize]
public class ConnectionHub : Hub
{
    private readonly ILogger<ConnectionHub> _logger;
    private string? UserIdString => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    private readonly IPresenceService _presenceService;

    public ConnectionHub(ILogger<ConnectionHub> logger, IPresenceService presenceService)
    {
        _logger = logger;
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        if (string.IsNullOrEmpty(UserIdString) || !int.TryParse(UserIdString, out var userID))
        {
            await base.OnConnectedAsync();
            return;
        }

        _logger.LogInformation("User '{UserId}' connected with ConnectionId {ConnectionId}", UserIdString,
            Context.ConnectionId);

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserIdString);
            await _presenceService.UpdateUserStatus(userID, true);
            await Clients.Others.SendAsync("UpdateUserStatus", new { UserId = UserIdString, IsOnline = true });
            _logger.LogInformation("User '{UserId}' added successfully to the group.", UserIdString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in added User to the group '{UserId}'", UserIdString);
            throw;
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (string.IsNullOrEmpty(UserIdString) || !int.TryParse(UserIdString, out var userID))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        if (exception != null)
            _logger.LogWarning(exception, "User '{UserId}' disconnected with error.", UserIdString);
        else
            _logger.LogWarning(exception, "User '{UserId}' disconnected.", UserIdString);

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserIdString);
            await _presenceService.UpdateUserStatus(userID, false);
            await Clients.Others.SendAsync("UpdateUserStatus", new { UserId = UserIdString, IsOnline = false });
            _logger.LogInformation("User '{UserId}' removed the group.", UserIdString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro removing user '{UserId}' from group.", UserIdString);
            throw;
        }

        await base.OnDisconnectedAsync(exception);
    }

}