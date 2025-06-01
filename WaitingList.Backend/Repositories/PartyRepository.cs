using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using WaitingListBackend.Database;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Repositories;

public class PartyRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IPartyRepository
{
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
            // No need to throw
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message, exception);
        }
    }

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