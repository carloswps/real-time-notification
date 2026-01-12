using System.Net;

namespace real_time_notification.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception caught by middleware. Request Path: {Path}",
                context.Request.Path);

            var statusCode = context.Response.StatusCode;
            if (statusCode < 400) statusCode = (int)HttpStatusCode.InternalServerError;

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    error = "error",
                    message = GetUserFriendlyErrorMessage(statusCode),
                    details = statusCode == (int)HttpStatusCode.InternalServerError
                        ? "An internal server error occurred. Please try again later."
                        : null,
                    statusCode = statusCode
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                _logger.LogWarning(
                    "Response already started, cannot write standardized error response for status code {StatusCode}. Exception: " +
                    "{ExceptionMessage}", statusCode, e.Message);
            }
        }
    }

    private string GetUserFriendlyErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            (int)HttpStatusCode.Unauthorized => "Authentication failed. Please check your credentials.",
            (int)HttpStatusCode.Forbidden => "You do not have permission to access this resource.",
            (int)HttpStatusCode.NotFound => "The requested resource was not found.",
            (int)HttpStatusCode.BadRequest => "The request was invalid. Please check your input.",
            (int)HttpStatusCode.InternalServerError => "An unexpected server error occurred. Please try again later.",
            _ => "An unknown error occurred."
        };
    }
}