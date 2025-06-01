using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    public ResultObject<PartyEntity> GetParty(string sessionId)
    {
       var result = new ResultObject<PartyEntity>();
       var party = _applicationDbContext.Parties.Include((x) => x.WaitingListEntity).SingleOrDefault((x) => x.SessionId == sessionId);
       if (party == null)
       {
           result.Messages.AddError($"Party not found");
       }
       else
       {
           result.Records.Add(party);
       }
       
       return result;   
    }
}