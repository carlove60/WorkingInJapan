using Microsoft.EntityFrameworkCore;
using WaitingListBackend.Database;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Repositories;

/// <summary>
/// Represents a repository for managing operations related to waiting lists.
/// Extends the functionality of the base repository and implements the
/// IWaitingListRepository interface.
/// </summary>
public class Repository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IWaitingListRepository
{
    /// <summary>
    /// Retrieves a waiting list by its name, optionally including checked-in parties.
    /// </summary>
    /// <param name="name">The name of the waiting list to retrieve.</param>
    /// <param name="includeCheckedIn">
    /// A boolean indicating whether to include parties that are checked-in.
    /// </param>
    /// <returns>
    /// A <see cref="ResultObject{T}"/> containing the requested waiting list entity
    /// and any relevant messages, including errors if the list is not found.
    /// </returns>
    public ResultObject<WaitingListEntity> GetWaitingList(string name, bool includeCheckedIn)
    {
        var result = new ResultObject<WaitingListEntity>();
        var waitingList = _applicationDbContext.WaitingLists
            .Include((x) => x.Parties.Where((p) => p.ServiceEndedAt == null && p.CheckedIn == includeCheckedIn))
            .SingleOrDefault((x) => x.Name == name);
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

    /// <summary>
    /// Retrieves a waiting list by its unique identifier, including all associated parties.
    /// </summary>
    /// <param name="id">The unique identifier of the waiting list to retrieve.</param>
    /// <returns>A <see cref="ResultObject{T}"/> containing the requested waiting list entity and any related messages.</returns>
    public ResultObject<WaitingListEntity> GetWaitingListWithAllParties(Guid id)
    {
        var result = new ResultObject<WaitingListEntity>();
        var waitingList = _applicationDbContext.WaitingLists.Include((x) => x.Parties).SingleOrDefault((x) => x.Id == id);
        if (waitingList == null)
        {
            result.Messages.AddError($"Waiting list with id {id} not found");
        }
        else
        {
            result.Records.Add(waitingList);
        }

        return result;    }
}