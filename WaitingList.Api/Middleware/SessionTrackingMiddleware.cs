using System.Collections.Concurrent;
using WaitingList.Extensions;

namespace WaitingList.Middleware;

/// <summary>
/// Middleware responsible for tracking active sessions within an application.
/// It monitors incoming requests, associates them with the current session,
/// and updates a central record of active sessions.
/// </summary>
public class SessionTrackingMiddleware
{
    /// <summary>
    /// Represents the next middleware in the pipeline to be executed after the current middleware.
    /// Responsible for forwarding the HTTP context to the subsequent middleware in the pipeline when invoked.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Maintains a thread-safe collection of active sessions, where each session is represented by a unique identifier and the corresponding last activity timestamp.
    /// Used by the middleware to track and update active sessions across incoming requests.
    /// </summary>
    private static readonly ConcurrentDictionary<string, DateTime> ActiveSessions = new();

    /// <summary>
    /// Middleware responsible for tracking and managing active sessions within the application.
    /// It updates the record of active sessions by associating incoming HTTP requests with session data.
    /// </summary>
    public SessionTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Handles the processing of the incoming HTTP context within the middleware.
    /// Updates the record of active sessions by retrieving or associating the session ID
    /// from the current request and invoking the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request.</param>
    /// <returns>A task representing the asynchronous operation of processing the HTTP request.</returns>
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

    /// <summary>
    /// Retrieves a read-only dictionary containing the currently active sessions.
    /// Each session is represented as a key-value pair, where the key is the session identifier
    /// and the value is the timestamp of the last activity.
    /// </summary>
    /// <returns>A read-only dictionary of active session identifiers and their last activity timestamps.</returns>
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
