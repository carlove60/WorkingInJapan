using WaitingList.DTO;
using WaitingList.Extensions;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Models;

namespace WaitingListBackend.Services;

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
    /// <param name="partyEntity"></param>
    /// <param name="waitingListId"></param>
    /// <returns></returns>
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyDto)
    {
        var response = new ResultObject<WaitingListDto>();

        var partyEntity = partyDto.ToEntity();
        
        var party = _partyRepository.AddParty(partyEntity);
        if (party.IsError)
        {
            response.Messages.AddRange(party.Messages);
            return response;       
        }

        // Ensure that multiple parties don't try to make a request at the same time and both reserve (a) seat(s)
        lock (this._waitingListRepository)
        {
           var waitingListResult =  _waitingListRepository.GetWaitingList(partyEntity.WaitingListId);
           if (waitingListResult.Messages.Count == 0 && waitingListResult.Records.Count > 0)
           {
               var waitingList = waitingListResult.Records.First();
               
               var hasSeatCapacity = WaitingListHasSeatCapacity(waitingList, partyEntity.Size);
               if (hasSeatCapacity)
               {
                   waitingList.Parties.Add(party.Records.First());
               }
               else
               {
                   response.Messages.AddError($"There are only {WaitinglistCapacityLeft(waitingList)} seats left in the waiting list. Please try again later.");
               }
           }
        }
        
        return response;
    }

    public ResultObject<WaitingListDto> CheckIn(PartyDto partyEntity)
    {
        throw new NotImplementedException();
    }

    public ResultObject<WaitingListDto> GetWaitingList(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public ResultObject<WaitingListDto> GetWaitingList(string name = Constants.DefaultWaitingListName)
    {
        var result = new ResultObject<WaitingListDto>();
        var waitingList = _waitingListRepository.GetWaitingList(name);
        result.Messages = waitingList.Messages;
        result.Records.Add(waitingList.Records.FirstOrDefault()?.ToDto());
        return result;
    }

    public ResultObject<WaitingListDto> GetMetaData()
    {
        var result = new ResultObject<WaitingListDto>();
        var waitingList = _waitingListRepository.GetWaitingList();
        result.Messages = waitingList.Messages;
        result.Records.Add(waitingList.Records.FirstOrDefault()?.ToDto());
        return result;
    }


    private bool WaitingListHasSeatCapacity(WaitingListEntity waitingList, int amountOfSeatsNeeded)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt != null);
        var amountOfSeatsTaken = parties.Sum((p) => p.Size);  
        return amountOfSeatsTaken + amountOfSeatsNeeded <= waitingList.TotalSeats;;
    }

    private int WaitinglistCapacityLeft(WaitingListEntity waitingList)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt != null);
        return parties.Sum((p) => p.Size);  
    }
}