namespace real_time_notification.Application.Interfaces;

public interface IPresenceService
{
    Task UpdateUserStatus(int userId, bool isOnline);
}