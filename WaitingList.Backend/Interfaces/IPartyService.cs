using WaitingList.DTO;

namespace WaitingListBackend.Interfaces;

public interface IPartyService
{
    ResultObject<PartyDto> GetParty(Guid sessionId);
}