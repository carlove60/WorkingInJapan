using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Services;

public class WaitingListService : IWaitingListService
{
    private IWaitingListRepository _waitingListRepository;
    private IPartyRepository _partyRepository;

    public WaitingListService(IWaitingListRepository waitingListRepository, IPartyRepository partyRepository)
    {
        this._waitingListRepository = waitingListRepository;
        this._partyRepository = partyRepository;
    }

    public AddToQueueResponse AddPartyToWaitingList(AddToQueueRequest request)
    {
        var response = new AddToQueueResponse();
        var party = _partyRepository.AddParty(request.Party);
        if (party.IsError)
        {
            response.Result.Messages.AddRange(party.Messages);
            response.Result.IsError = party.IsError;
            return response;       
        }

        // Ensure that multiple parties don't try to make a request at the same time and both reserve (a) seat(s)
        lock (this._waitingListRepository)
        {
           var waitingListResult =  _waitingListRepository.GetWaitingList(request.WaitingListId);
           if (waitingListResult.Messages.Count == 0 && waitingListResult.Records.Count > 0)
           {
               var waitingList = waitingListResult.Records.First();
               waitingList.Parties.Add(party.Records.First());
           }
        }
        
        return response;
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