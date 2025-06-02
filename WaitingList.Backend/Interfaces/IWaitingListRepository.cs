using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Defines operations related to the management and retrieval of waiting list entities.
/// Provides methods for querying waiting lists by name and including additional details.
/// </summary>
public interface IWaitingListRepository
{
    ResultObject<WaitingListEntity> GetWaitingList(string name, bool includeCheckedIn);
    ResultObject<WaitingListEntity> GetWaitingListWithAllParties(Guid id);
}