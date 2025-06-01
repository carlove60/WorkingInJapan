using WaitingList.Contracts.DTOs;

namespace WaitingListBackend.Interfaces;

public interface IWaitingListService
{
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyEntity);
    
    public ResultObject<WaitingListDto> GetWaitingList(string name);
}