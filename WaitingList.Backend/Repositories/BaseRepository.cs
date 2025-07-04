using WaitingList.Database.Database;
using WaitingList.SseManager.Managers;

namespace WaitingListBackend.Repositories;

/// <summary>
/// BaseRepository serves as an abstract base class for all repository classes in the application,
/// providing shared behavior and access to the application database context.
/// </summary>
public class BaseRepository
{
    /// <summary>
    /// Represents a reference to the application's database context,
    /// providing access to database entities and operations through Entity Framework.
    /// </summary>
    protected readonly ApplicationDbContext ApplicationDbContext;

    /// <summary>
    /// Represents a manager for handling Server-Sent Events (SSE) channels,
    /// enabling the creation, management, and broadcasting of messages to
    /// client-specific channels within the application.
    /// </summary>
    protected readonly SseChannelManager SseChannelManager;

    /// <summary>
    /// The BaseRepository class provides a foundational implementation for creating repository classes.
    /// It handles the integration with the application's database context, enabling derived repository classes to interact with the database through shared functionality.
    /// </summary>
    public BaseRepository(ApplicationDbContext applicationDbContext, SseChannelManager sseChannelManager)
    {
        ApplicationDbContext = applicationDbContext;
        SseChannelManager = sseChannelManager;
    }
}