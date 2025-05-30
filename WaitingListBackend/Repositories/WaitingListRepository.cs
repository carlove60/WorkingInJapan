using WaitingListBackend.Database;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Repositories;

public class WaitingListRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IWaitingListRepository
{
    public ResultObject<WaitingListEntity> GetWaitingList(string name = Constants.DefaultWaitingListName)
    {
        var result = new ResultObject<WaitingListEntity>();
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

    public ResultObject<WaitingListEntity> GetWaitingList(Guid id)
    {
        var result = new ResultObject<WaitingListEntity>();
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