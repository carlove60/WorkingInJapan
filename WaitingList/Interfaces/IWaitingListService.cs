using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Interfaces;

public interface IWaitingListService
{
    AddToQueueResponse AddPartyToWaitingList(AddToQueueRequest request);
    CheckInResponse CheckIn(CheckInRequest request);
    
    WaitingListMetaDataResponse GetMetaData();
}