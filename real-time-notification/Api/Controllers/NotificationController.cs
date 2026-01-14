using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using real_time_notification.Application.DTO;
using real_time_notification.Application.Interfaces;

namespace real_time_notification.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/notification")]
[ApiVersion("1.0")]
[Authorize]
public class NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationController> _logger = logger;

    [HttpPost("send-message/{userId}")]
    public async Task<IActionResult> SendMessageAsync(string userId, [FromBody] NotificationRequestDto message)
    {
        await _notificationService.SendNotificationAsync(message.Message, userId, message.Tittle);
        return Ok(new { message = "Message sent successfully." });
    }

    [HttpGet("get-user-status/{userId}")]
    public async Task<IActionResult> GetUserStatus(int userId)
    {
        await _notificationService.GetUserStatus(userId);
        return Ok(new { message = "User status retrieved successfully." });
    }
}