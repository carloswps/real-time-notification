namespace real_time_notification.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string message, string userId, string title);
}