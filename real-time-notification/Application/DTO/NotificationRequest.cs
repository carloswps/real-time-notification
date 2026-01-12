using System.ComponentModel.DataAnnotations;

namespace real_time_notification.Application.DTO;

public class NotificationRequest
{
    [Required] public string Message { get; set; } = "";

    [Required] public string Tittle { get; set; } = "";
}