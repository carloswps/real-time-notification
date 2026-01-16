using Microsoft.AspNetCore.SignalR;

namespace real_time_notification.Api.Hubs.Filters;

public class HubErrorFilter : IHubFilter
{
    private readonly ILogger<HubErrorFilter> _logger;

    public HubErrorFilter(ILogger<HubErrorFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in method Hub: {MethodName}", invocationContext.HubMethodName);
            throw new HubException("Error in real-time-notification Server");
        }
    }
}