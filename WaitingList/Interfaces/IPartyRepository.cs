using WaitingList.Entities;
using WaitingList.Models;
using WaitingList.Requests;

namespace WaitingList.Interfaces;

public interface IPartyRepository
{
    ResultObject<PartyEntity> AddParty(PartyEntity request);
    
    ResultObject<PartyEntity> GetParty(Guid id);
}