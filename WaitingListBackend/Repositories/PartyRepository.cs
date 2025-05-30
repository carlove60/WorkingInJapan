using WaitingListBackend.Database;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Repositories;

internal class PartyRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IPartyRepository
{
    public ResultObject<PartyEntity> AddParty(PartyEntity request)
    {
        var result = new ResultObject<PartyEntity>();
        try
        {
            var newParty = new PartyEntity
            {
                Name = request.Name,
                Size = request.Size
            };

            var saveResult = _applicationDbContext.Parties.Add(newParty);
            _applicationDbContext.SaveChanges();
            
            result.Records.Add(saveResult.Entity);
        }
        catch (Exception exception)
        {
            result.Messages.AddError(exception.Message);       
        }

        return result;
    }

    public ResultObject<PartyEntity> GetParty(Guid id)
    {
       var result = new ResultObject<PartyEntity>();
       var party = _applicationDbContext.Parties.SingleOrDefault((x) => x.Id == id);
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