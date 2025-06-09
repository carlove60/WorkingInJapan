using WaitingList.Contracts.DTOs;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Provides services for managing a waiting list, including operations
/// for adding parties to the waiting list and retrieving waiting list data.
/// </summary>
public interface IWaitingListService
{
    /// <summary>
    /// Adds a party to the waiting list.
    /// </summary>
    /// <param name="partyEntity">The party details, including name, size, waiting list name, and session ID.</param>
    /// <returns>A result object containing information about the operation, including the updated waiting list data and any messages.</returns>
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyEntity);

    /// <summary>
    /// Retrieves the waiting list based on the provided name.
    /// </summary>
    /// <param name="name">The name of the waiting list to retrieve.</param>
    /// <returns>A result object containing the waiting list data and any associated messages.</returns>
    public ResultObject<WaitingListDto> GetWaitingList(string name);
}