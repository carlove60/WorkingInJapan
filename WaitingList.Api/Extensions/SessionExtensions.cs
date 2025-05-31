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
    public static string GetOrCreateSessionId(this ISession session)
    {
        var sessionId = session.GetString(Constants.WaitingListSessionKey);
        if (!string.IsNullOrEmpty(sessionId))
        {
            return sessionId;
        }
        
        sessionId = Guid.NewGuid().ToString();
        session.SetString(Constants.WaitingListSessionKey, sessionId);
        return sessionId;
    }
}