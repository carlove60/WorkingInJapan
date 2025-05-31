using WaitingList.Extensions;
using WaitingListBackend;

namespace WaitingList.Middleware;

/// <summary>
/// 
/// </summary>
public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        var sessionId = context.Session.GetString(Constants.WaitingListSessionKey);
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = context.Session.GetOrCreateSessionId();
        }
        
        // May throw, but acceptable if the header already exists.
        context.Response.Headers.Add("X-Session-Id", sessionId);
        
        await _next(context);
    }
}

/// <summary>
/// Provides extension methods for SessionValidationMiddleware to simplify its integration into an application's request pipeline.
/// </summary>
public static class SessionValidationMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="SessionValidationMiddleware"/> to the application's request pipeline.
    /// This middleware is responsible for validating session-related data in incoming HTTP requests.
    /// </summary>
    /// <param name="builder">The application builder used to configure the middleware pipeline.</param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> instance with the <see cref="SessionValidationMiddleware"/> added.
    /// </returns>
    public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionValidationMiddleware>();
    }
}