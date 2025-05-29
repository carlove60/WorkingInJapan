using WaitingList.Database;
using WaitingList.Interfaces;
using WaitingList.Models;

namespace WaitingList.Repositories;

public class WaitingListRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IWaitingListRepository 
{
    public ResultObject<WaitingListModel> GetWaitingList(string name = Constants.DefaultWaitingListName)
    {
        var result = new ResultObject<WaitingListModel>();
        var waitingList = _applicationDbContext.WaitingLists.SingleOrDefault((x) => x.Name == name);
        if (waitingList == null)
        {
            result.Messages.AddError($"{name} not found");
        }
        else
        {
            result.Records.Add(waitingList);
        }

        return result;
    }

    public ResultObject<WaitingListModel> GetWaitingList(Guid id)
    {
        var result = new ResultObject<WaitingListModel>();
        var waitingList = _applicationDbContext.WaitingLists.SingleOrDefault((x) => x.Id == id);
        if (waitingList == null)
        {
            result.Messages.AddError($"WaitingList not found");
        }
        else
        {
            result.Records.Add(waitingList);
        }

        return result;
    }
}