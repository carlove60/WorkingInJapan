using System.Collections.Concurrent;
using WaitingList.Extensions;

namespace WaitingList.Middleware;

public class SessionTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, DateTime> ActiveSessions = new();

    public SessionTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Ensure the session is loaded
        await context.Session.LoadAsync();

        var sessionId = context.Session.GetSessionId();

        if (!string.IsNullOrEmpty(sessionId))
        {
            ActiveSessions[sessionId] = DateTime.Now;
        }

        await _next(context);
    }

    public static IReadOnlyDictionary<string, DateTime> GetActiveSessions() => ActiveSessions;
}

/// <summary>
/// Provides extension methods for SessionValidationMiddleware to simplify its integration into an application's request pipeline.
/// </summary>
public static class SessionTrackingMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="SessionMiddleware"/> to the application's request pipeline.
    /// This middleware is responsible for validating session-related data in incoming HTTP requests.
    /// </summary>
    /// <param name="builder">The application builder used to configure the middleware pipeline.</param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> instance with the <see cref="SessionMiddleware"/> added.
    /// </returns>
    public static IApplicationBuilder useSessionTrackingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionTrackingMiddleware>();
    }
}
