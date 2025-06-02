using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IWaitingListRepository
{
    ResultObject<WaitingListEntity> GetWaitingList(string name, bool includeCheckedIn);
    ResultObject<WaitingListEntity> GetWaitingListWithAllParties(Guid id);
}