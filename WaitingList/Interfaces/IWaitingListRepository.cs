using WaitingList.Models;

namespace WaitingList.Interfaces;

public interface IWaitingListRepository
{
    ResultObject<WaitingListModel> GetWaitingList(string name = Constants.DefaultWaitingListName);
    ResultObject<WaitingListModel> GetWaitingList(Guid id);
}