using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using real_time_notification.Application.DTO;
using real_time_notification.Application.Interfaces;

namespace real_time_notification.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/notification")]
[ApiVersion("1.0")]
public class NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationController> _logger = logger;

    [HttpPost("send-message/{userId}")]
    public async Task<IActionResult> SendMessageAsync(string userId, [FromBody] NotificationRequest message)
    {
        try
        {
            await _notificationService.SendNotificationAsync(userId, "Message Teste", message.Message);
            return Ok(new { message = "Mensagem enviada com sucesso" });
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error in SendMessageAsync:", e);
            throw;
        }
    }
}