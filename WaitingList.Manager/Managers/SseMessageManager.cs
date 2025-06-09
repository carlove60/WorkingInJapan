using Microsoft.Extensions.Logging;
using WaitingList.Contracts.DTOs;

namespace WaitingList.SseManager.Managers;

/// <summary>
/// A manager responsible for handling server-sent events (SSE) messages, managing updates for parties and waiting lists,
/// and providing an interface for sending SSE messages.
/// </summary>
public class SseMessageManager
{
    /// <summary>
    /// Holds a collection of party data transfer objects (DTOs) marked for updates.
    /// This property is used to track parties needing to be processed and relayed to clients
    /// via server-sent events (SSE). Ensures proper management of updates to party data.
    /// </summary>
    private HashSet<PartyDto> PartiesToUpdate { get; set; } = new();

    /// <summary>
    /// Maintains a collection of waiting lists that are scheduled for updates.
    /// This property is used to queue waiting list data transfer objects (DTOs)
    /// to be processed and broadcast to clients via server-sent events (SSE).
    /// Ensures proper tracking and handling of updates to waiting list data.
    /// </summary>
    private HashSet<WaitingListDto> WaitingListsToUpdate { get; set; } = new();

    /// <summary>
    /// Handles the management of server-sent events (SSE) channels, including creation, broadcast,
    /// targeted message sending, and cleanup of communication channels associated with client sessions.
    /// Facilitates the flow of event-driven messages between the server and clients.
    /// </summary>
    private readonly SseChannelManager _sseChannelManager;

    /// <summary>
    /// Logger instance used for logging informational, warning, error, and debug messages
    /// within the SseMessageManager class to aid in diagnostics and application flow tracking.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Manages server-sent events (SSE) messages and interactions.
    /// </summary>
    public SseMessageManager(SseChannelManager sseChannelManager, ILogger logger)
    {
        _sseChannelManager = sseChannelManager;
        _logger = logger;
    }

    /// <summary>
    /// Adds a party to the collection of parties that are marked for updates.
    /// </summary>
    /// <param name="party">
    /// The party data transfer object (DTO) to be added for processing updates.
    /// </param>
    public void AddParty(PartyDto party)
    {
        PartiesToUpdate.Add(party);
    }

    /// <summary>
    /// Adds a waiting list to the collection of waiting lists that are marked for updates.
    /// </summary>
    /// <param name="waitingList">
    /// The waiting list data transfer object (DTO) to be added for processing updates.
    /// </param>
    public void AddWaitingList(WaitingListDto waitingList)
    {
        WaitingListsToUpdate.Add(waitingList);  
    }

    /// <summary>
    /// Sends queued SSE (Server-Sent Events) messages for updates to parties and waiting lists,
    /// broadcasting the relevant data through the associated SSE channels, and then clears the update queues.
    /// </summary>
    /// <remarks>
    /// This method processes the updates by iterating through parties and waiting lists that need to be updated.
    /// For each party, an SSE message is created and sent to the connected session identified by their session ID.
    /// Similarly, updates for waiting lists are broadcasted.
    /// After all updates are sent, the method clears the internal lists of parties and waiting lists to avoid redundant processing.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the SSE channel manager or logging utilities are not properly initialized.
    /// </exception>
    /// <seealso cref="SseDto{T}"/>
    /// <seealso cref="SseChannelManager"/>
    public void SendMessagesAndClear()
    {
       foreach (var party in PartiesToUpdate)
       {
           var sseDto = new SseDto<PartyDto>(party, nameof(PartyDto));
           _sseChannelManager.SendDto(party.SessionId, sseDto);
           _logger.LogInformation($"Party: {party.Name} messaged.");
       }
       foreach (var waitingList in WaitingListsToUpdate)
       {
           _logger.LogInformation($"Waiting list: {waitingList.Name} updated.");
           var sseDto = new SseDto<WaitingListDto>(waitingList, nameof(WaitingListDto));
           _sseChannelManager.BroadcastDto(sseDto);
       };
        
       PartiesToUpdate.Clear();
       WaitingListsToUpdate.Clear();
    }
}