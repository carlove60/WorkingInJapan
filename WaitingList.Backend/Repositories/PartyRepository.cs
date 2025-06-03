using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WaitingList.Database.Database;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Repositories;

/// <summary>
/// Provides methods for managing and interacting with party data in the database.
/// </summary>
public class PartyRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IPartyRepository
{
    /// <summary>
    /// Saves a party entity to the database. If the entity does not exist, it will be added; otherwise, it will be updated.
    /// </summary>
    /// <param name="party">The party entity to be saved.</param>
    /// <returns>A result object that contains the saved party entity and any related messages.</returns>
    public ResultObject<PartyEntity> SaveParty(PartyEntity party)
    {
        var resultObject = new ResultObject<PartyEntity>();
        try
        {
            EntityEntry<PartyEntity> partyEntry;
            if (party.Id  == Guid.Empty)
            {
                partyEntry = _applicationDbContext.Parties.Add(party);
            }
            else
            {
                partyEntry = _applicationDbContext.Parties.Update(party);
            }

            _applicationDbContext.SaveChanges();
            
            resultObject.Records.Add(partyEntry.Entity);
        }
        catch (Exception exception)
        {
            resultObject.Messages.AddError(exception.Message);       
        }

        return resultObject;
    }

    /// <summary>
    /// Removes a party entity from the repository and commits the changes to the database.
    /// Possible to get an exception where the record has already been deleted by the background service
    /// </summary>
    /// <param name="party">The party entity to be removed.</param>
    public void RemoveParty(PartyEntity party)
    {
        _applicationDbContext.Parties.Remove(party);
        try
        {
            _applicationDbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            // No need to throw, background task has deleted this one
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message, exception);
        }
    }

    /// <summary>
    /// Retrieves a party entity by the given session ID from the database, including its associated waiting list entity.
    /// </summary>
    /// <param name="sessionId">The session ID associated with the party to retrieve.</param>
    /// <returns>A result object containing the matching party entity, if found, along with any associated messages.</returns>
    public ResultObject<PartyEntity> GetParty(string sessionId)
    {
       var result = new ResultObject<PartyEntity>();
       var party = _applicationDbContext.Parties.Include((x) => x.WaitingListEntity).SingleOrDefault((x) => x.SessionId == sessionId);
       if (party != null)
       {
           result.Records.Add(party);
       }

       return result;   
    }
}