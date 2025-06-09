using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Interface for managing and interacting with party-related operations in the system.
/// </summary>
public interface IPartyService
{
    /// <summary>
    /// Retrieves the party information associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the party information is requested.</param>
    /// <returns>
    /// A <see cref="ResultObject{PartyDto}"/> containing the party information.
    /// If no party is found for the given session ID, the result may contain an empty or null record.
    /// </returns>
    ResultObject<PartyDto> GetParty(string sessionId);

    /// <summary>
    /// Marks the party associated with the specified session ID as checked in.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the party check-in is performed.</param>
    /// <returns>
    /// A <see cref="ResultObject{PartyDto}"/> containing the result of the check-in operation.
    /// If the session ID is invalid or no party is found, the result may contain error messages.
    /// </returns>
    ResultObject<PartyDto> CheckIn(string sessionId);

    /// <summary>
    /// Cancels the check-in process for the party associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the check-in is to be canceled.</param>
    /// <returns>
    /// A <see cref="ResultObject{PartyDto}"/> indicating the outcome of the cancellation.
    /// If no party exists for the given session ID, the result contains an error message.
    /// </returns>
    ResultObject<PartyDto> CancelCheckIn(string sessionId);

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
    /// Retrieves the next party eligible for check-in from the provided list of parties.
    /// </summary>
    /// <param name="parties">The collection of party entities to evaluate for the next eligible party to check in.</param>
    /// <returns>
    /// The next party entity that is not yet checked in, ordered by the earliest creation date, or null if no such party exists.
    /// </returns>
    PartyEntity? GetNextPartyToCheckIn(IEnumerable<PartyEntity> parties);
}