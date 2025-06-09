using WaitingList.Database.Entities;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Defines operations related to the management and retrieval of waiting list entities.
/// Provides methods for querying waiting lists by name and including additional details.
/// </summary>
public interface IWaitingListRepository
{
    /// <summary>
    /// Retrieves the waiting list by name, optionally including checked-in parties.
    /// </summary>
    /// <param name="name">The name of the waiting list to retrieve.</param>
    /// <param name="includeCheckedIn">A boolean value indicating whether to include checked-in parties in the result.</param>
    /// <returns>A ResultObject containing the WaitingListEntity records and associated messages.</returns>
    ResultObject<WaitingListEntity> GetWaitingList(string name, bool includeCheckedIn);

    /// <summary>
    /// Retrieves the waiting list along with all associated parties for the specified waiting list identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the waiting list.</param>
    /// <returns>A ResultObject containing the WaitingListEntity records with all associated parties and related messages.</returns>
    ResultObject<WaitingListEntity> GetWaitingListWithAllParties(Guid id);
}