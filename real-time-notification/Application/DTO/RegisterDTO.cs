using System;
using System.ComponentModel.DataAnnotations;

namespace real_time_notification.Application.DTO;

public class RegisterDTO
{
    [Required] public string Email { get; set; } = "";

    [Required] public string Password { get; set; } = "";
}
