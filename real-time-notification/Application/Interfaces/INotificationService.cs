using real_time_notification.Application.DTO;

namespace real_time_notification.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string message, string userId, string title);
    Task<UserStatusDto> GetUserStatus(int userId);
}