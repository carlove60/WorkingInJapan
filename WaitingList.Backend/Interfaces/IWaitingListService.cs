using WaitingList.DTO;
using WaitingListBackend.Entities;
using WaitingListBackend.Models;

namespace WaitingListBackend.Interfaces;

public interface IWaitingListService
{
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyEntity);
    
    public ResultObject<WaitingListDto> CheckIn(PartyDto partyEntity);
    
    public ResultObject<WaitingListDto> GetWaitingList(string name = Constants.DefaultWaitingListName);
}