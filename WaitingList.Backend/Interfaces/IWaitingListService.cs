using WaitingList.Contracts.DTOs;

namespace WaitingListBackend.Interfaces;

/// <summary>
/// Provides services for managing a waiting list, including operations
/// for adding parties to the waiting list and retrieving waiting list data.
/// </summary>
public interface IWaitingListService
{
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyEntity);
    
    public ResultObject<WaitingListDto> GetWaitingList(string name);
}