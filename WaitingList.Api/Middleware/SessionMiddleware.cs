using WaitingList.Extensions;

namespace WaitingList.Middleware;

/// <summary>
/// Middleware that processes HTTP requests and responses to ensure a session ID is associated with the current session
/// and included in the response headers.
/// </summary>
public class SessionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to process HTTP requests and responses,
    /// ensuring that a session ID is associated with the current session and added to the response headers.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
    /// <returns>
    /// A <see cref="Task"/> that represents the execution of the middleware.
    /// </returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var sessionId = context.Session.GetString(WaitingListBackend.Constants.WaitingListSessionKey);
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = context.Session.CreateSessionId();
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
    /// Adds the <see cref="SessionMiddleware"/> to the application's request pipeline.
    /// This middleware is responsible for validating session-related data in incoming HTTP requests.
    /// </summary>
    /// <param name="builder">The application builder used to configure the middleware pipeline.</param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> instance with the <see cref="SessionMiddleware"/> added.
    /// </returns>
    public static IApplicationBuilder UseSessionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionMiddleware>();
    }
}