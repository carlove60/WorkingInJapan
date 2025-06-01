using WaitingList.Contracts.DTOs;
using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IPartyService
{
    ResultObject<PartyDto> GetParty(string sessionId);
    
    ResultObject<PartyDto> CheckIn(string sessionId);
}