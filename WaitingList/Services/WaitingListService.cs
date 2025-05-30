using WaitingList.Entities;
using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Services;

/// <summary>
/// 
/// </summary>
public class WaitingListService : IWaitingListService
{
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly IPartyRepository _partyRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitingListRepository"></param>
    /// <param name="partyRepository"></param>
    public WaitingListService(IWaitingListRepository waitingListRepository, IPartyRepository partyRepository)
    {
        _waitingListRepository = waitingListRepository;
        _partyRepository = partyRepository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public AddToWaitingListResponse AddPartyToWaitingList(AddToWaitingListRequest request)
    {
        var response = new AddToWaitingListResponse();
        var partyModel = new PartyEntity { Name = request.PartyName, Size = request.PartySize};
        var party = _partyRepository.AddParty(partyModel);
        if (party.IsError)
        {
            response.Messages.AddRange(party.Messages);
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
        var result = new WaitingListMetaDataResponse();
        var waitingListResult = _waitingListRepository.GetWaitingList();
        result.Messages.AddRange(waitingListResult.Messages);       
        if (!waitingListResult.IsError && waitingListResult.Records.Count > 0)
        {
            var waitingList = waitingListResult.Records.Single();
            result.TotalSeatsAvailable = waitingList.TotalSeatsAvailable;
            result.WaitingListName = waitingList.Name;       
        }

        return result;
    }

    public PartyEntity CheckIn(PartyRequest request)
    {
        throw new NotImplementedException();
    }
}