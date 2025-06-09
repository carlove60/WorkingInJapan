using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;

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
    
    /// <summary>
    /// Determines whether the party associated with the specified session ID is eligible for check-in.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session to evaluate for check-in eligibility.</param>
    /// <returns>
    /// A <see cref="ResultObject{T}"/> containing a boolean value indicating whether the party can check in.
    /// True if the party is eligible, otherwise false.
    /// </returns>
    ResultObject<bool> CanCheckIn(string sessionId);

    /// <summary>
    /// Retrieves the next party from the waiting list that is eligible to check in,
    /// based on the order of creation and ensuring they have not already checked in.
    /// </summary>
    /// <param name="waitingListEntity">The waiting list entity containing the parties and their details.</param>
    /// <returns>The next party eligible for check-in as a PartyDto, or null if no eligible party exists.</returns>
    PartyDto? GetNextPartyToCheckIn(WaitingListEntity waitingListEntity);
}