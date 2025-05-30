using WaitingListBackend.Entities;

namespace WaitingListBackend.Interfaces;

public interface IWaitingListService
{
    public ResultObject<WaitingListEntity> AddPartyToWaitingList(PartyEntity partyEntity, Guid waitingListId);
    
    public ResultObject<WaitingListEntity> CheckIn(Guid waitingListId, Guid partyId);

    public ResultObject<WaitingListEntity> GetMetaData();
}