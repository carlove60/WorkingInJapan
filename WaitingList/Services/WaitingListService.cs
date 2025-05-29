using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Services;

public class WaitingListService : IWaitingListService
{
    private IWaitingListRepository _waitingListRepository;

    public WaitingListService(IWaitingListRepository waitingListRepository)
    {
        this._waitingListRepository = waitingListRepository;
    }

    public AddToQueueResponse AddPartyToWaitingList(AddToQueueRequest request)
    {
        throw new NotImplementedException();
    }

    public CheckInResponse CheckIn(CheckInRequest request)
    {
        throw new NotImplementedException();
    }

    public WaitingListMetaDataResponse GetMetaData()
    {
        var result = this._waitingListRepository.GetWaitingList();
        return new WaitingListMetaDataResponse { Result = result };
    }

    public PartyModel CheckIn(PartyRequest request)
    {
        throw new NotImplementedException();
    }
}