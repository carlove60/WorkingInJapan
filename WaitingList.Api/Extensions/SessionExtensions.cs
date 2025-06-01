using WaitingListBackend;

namespace WaitingList.Extensions;

/// <summary>
/// Provides extension methods for working with the .Net Core useSession middleware
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Retrieves the existing session ID from the session, or generates a new one if no session ID exists.
    /// </summary>
    /// <param name="session">The session where the session ID is stored or created.</param>
    /// <returns>The session ID as a string. If no session ID exists, a new one is created, stored, and returned.</returns>
    public static string? GetSessionId(this ISession session)
    {
        return session.GetString(WaitingListBackend.Constants.WaitingListSessionKey);
    }

    /// <summary>
    /// Creates a new session ID, stores it in the provided session, and returns it.
    /// </summary>
    /// <param name="session">The session where the new session ID is stored.</param>
    /// <returns>The newly created session ID as a string.</returns>
    public static string CreateSessionId(this ISession session)
    {
        var sessionId = Guid.NewGuid().ToString();
        session.SetString(WaitingListBackend.Constants.WaitingListSessionKey, sessionId);
        return sessionId;
    }
}