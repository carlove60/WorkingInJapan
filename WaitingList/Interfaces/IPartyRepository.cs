using WaitingList.Models;
using WaitingList.Requests;

namespace WaitingList.Interfaces;

public interface IPartyRepository
{
    ResultObject<PartyModel> AddParty(PartyModel request);
    
    ResultObject<PartyModel> GetParty(Guid id);
}