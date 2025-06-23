using WaitingList.Database.Entities;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Provides an interface for managing party data, including saving, retrieving, and removing party entities.
/// </summary>
public interface IPartyRepository
{
    /// <summary>
    /// Saves the specified party entity to the repository.
    /// </summary>
    /// <param name="party">The party entity to be saved.</param>
    /// <returns>
    /// A result object containing the saved party entity and any messages providing details about the operation or issues encountered.
    /// </returns>
    ResultObject<PartyEntity> SaveParty(PartyEntity party);

    /// <summary>
    /// Retrieves the party entity associated with the specified session identifier.
    /// </summary>
    /// <param name="sessionId">The unique identifier for the session to retrieve the associated party entity.</param>
    /// <returns>
    /// A result object containing the party entity and any messages providing details about the retrieval operation or issues encountered.
    /// </returns>
    ResultObject<PartyEntity> GetParty(string sessionId);

    /// <summary>
    /// Removes the specified party entity from the repository.
    /// </summary>
    /// <param name="party">The party entity to be removed.</param>
    /// <returns>
    /// A result object containing the removed party entity, and potential messages indicating the status or any issues encountered.
    /// </returns>
    ResultObject<PartyEntity> RemoveParty(PartyEntity party);
}