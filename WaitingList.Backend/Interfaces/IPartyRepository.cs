using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IPartyRepository
{
    ResultObject<PartyEntity> SaveParty(PartyEntity party);
    
    ResultObject<PartyEntity> GetParty(string sessionId);

    void RemoveParty(PartyEntity party);
}