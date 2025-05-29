using WaitingList.Database;
using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;

namespace WaitingList.Repositories;

public class PartyRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext), IPartyRepository
{
    public ResultObject<PartyModel> AddParty(PartyModel request)
    {
        var result = new ResultObject<PartyModel>();
        try
        {
            var newParty = new PartyModel
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

    public ResultObject<PartyModel> GetParty(Guid id)
    {
       var result = new ResultObject<PartyModel>();
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