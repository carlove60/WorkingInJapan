using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IWaitingListRepository
{
    ResultObject<WaitingListEntity> GetWaitingList(string name = Constants.DefaultWaitingListName);
    ResultObject<WaitingListEntity> GetWaitingList(Guid id);
}