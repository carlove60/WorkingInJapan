using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IPartyRepository
{
    ResultObject<PartyEntity> AddParty(PartyEntity request);
    
    ResultObject<PartyEntity> GetParty(Guid sessionId);
}