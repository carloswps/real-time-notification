using real_time_notification.Application.Interfaces;
using real_time_notification.Infra;

namespace real_time_notification.Application.Services;

public class PresenceService(ILogger<PresenceService> logger, AppDbContext context) : IPresenceService
{
    private readonly ILogger<PresenceService> _logger = logger;
    private readonly AppDbContext _context = context;

    public async Task UpdateUserStatus(int userId, bool isOnline)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsOnline = isOnline;
            user.LastOnline = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}