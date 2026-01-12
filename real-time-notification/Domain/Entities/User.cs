using System;
using System.ComponentModel.DataAnnotations;

namespace real_time_notification.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsOnline { get; set; } = false;
    public DateTime? LastOnline { get; set; }
}
