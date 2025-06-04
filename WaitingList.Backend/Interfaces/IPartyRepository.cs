using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Provides an interface for managing party data, including saving, retrieving, and removing party entities.
/// </summary>
public interface IPartyRepository
{
    ResultObject<PartyEntity> SaveParty(PartyEntity party);
    
    ResultObject<PartyEntity> GetParty(string sessionId);

    ResultObject<PartyEntity> RemoveParty(PartyEntity party);
}