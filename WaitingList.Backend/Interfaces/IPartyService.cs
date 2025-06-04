using WaitingList.Contracts.DTOs;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Interface for managing and interacting with party-related operations in the system.
/// </summary>
public interface IPartyService
{
    ResultObject<PartyDto> GetParty(string sessionId);
    
    ResultObject<PartyDto> CheckIn(string sessionId);
    
    ResultObject<PartyDto> CancelCheckIn(string sessionId);
}