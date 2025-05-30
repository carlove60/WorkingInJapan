using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Interfaces;

public interface IWaitingListService
{
    AddToWaitingListResponse AddPartyToWaitingList(AddToWaitingListRequest request);
    CheckInResponse CheckIn(CheckInRequest request);
    
    WaitingListMetaDataResponse GetMetaData();
}