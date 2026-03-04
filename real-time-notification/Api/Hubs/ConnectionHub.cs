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

    private int UserId => int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    private string? UserIdString => UserId.ToString();
    private readonly IPresenceService _presenceService;

    public ConnectionHub(ILogger<ConnectionHub> logger, IPresenceService presenceService)
    {
        _logger = logger;
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userID = UserId;
        if (userID == 0)
        {
            _logger.LogWarning("User not authenticated.");
            return;
        }

        if (string.IsNullOrEmpty(UserIdString))
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
        var userID = UserId;
        if (userID != 0)
        {
            try
            {
                await _presenceService.UpdateUserStatus(userID, false);
                await Clients.Others.SendAsync("UpdateUserStatus", new { UserId = UserIdString, IsOnline = false });
                _logger.LogInformation("User '{UserId}' removed the group.", UserIdString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro removing user '{UserId}' from group.", UserIdString);
                throw;
            }
        }
            
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinPrivateChat(int targetUserId)
    {
        if (UserId == 0)
        {
            _logger.LogWarning("User not authenticated.");
            return;
        }

        var roomId = GetRoom(UserId, targetUserId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendMessage(int targetUserId, string message)
    {
        if (UserId == 0)
        {
            _logger.LogWarning("User not authenticated.");
            return;
        }

        var roomId = GetRoom(UserId, targetUserId);
        await Clients.Group(roomId).SendAsync("ReceivePrivateMessage", new
        {
            SenderId = UserId,
            Content = message,
            Timestamp = DateTime.Now.Hour
        });
    }

    private string GetRoom(int id1, int id2)
    {
        return id1 < id2 ? $"chat_{id1}_{id2}" : $"chat_{id2}_{id1}";
    }
}