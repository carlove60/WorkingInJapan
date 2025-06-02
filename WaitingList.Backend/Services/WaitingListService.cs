using WaitingList.Contracts.DTOs;
using WaitingList.Extensions;
using WaitingListBackend.Entities;
using WaitingListBackend.Extensions;
using WaitingListBackend.Interfaces;

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
    /// Adds a party to the waiting list if there is sufficient capacity.
    /// </summary>
    /// <param name="partyDto">The data transfer object representing the party to be added to the waiting list.</param>
    /// <returns>A result object containing a WaitingListDto and any messages, such as errors or status updates, resulting from the operation.</returns>
    public ResultObject<WaitingListDto> AddPartyToWaitingList(PartyDto partyDto)
    {
        var response = new ResultObject<WaitingListDto>();
        
        // Ensure that multiple parties don't try to make a request at the same time and both reserve (a) seat(s)
        lock (this._waitingListRepository)
        {
            var waitingListResult =  _waitingListRepository.GetWaitingList(partyDto.WaitingListName, false);
            response.Messages.AddRange(waitingListResult.Messages);
            if (waitingListResult.Records.Count == 0)
            {
                return response;
            }

            var waitingList = waitingListResult.Records.First();
            
            var existingParty = waitingList.Parties.FirstOrDefault((p) => p.SessionId == partyDto.SessionId && p.ServiceStartedAt == null);;
            if (existingParty != null)
            {
                response.Records.Add(waitingList.ToDto());
                return response;  
            }

            var hasSeatCapacity = WaitingListHasSeatCapacity(waitingList, partyDto.Size);
            if (!hasSeatCapacity)
            {
                response.Messages.AddError(
                    $"There are only {CalculateSeatsAvailable(waitingList)} seat(s) left on the waiting list. Please try again later.");
                return response;
            }

            var partyEntity = partyDto.ToEntity();
            partyEntity.WaitingListId = waitingList.Id;
        
            var party = _partyRepository.SaveParty(partyEntity);
            if (party.IsError)
            {
                response.Messages.AddRange(party.Messages);
                return response;       
            }
        }
        
        return response;
    }
    
    public ResultObject<WaitingListDto> GetWaitingList(string name)
    {
        var result = new ResultObject<WaitingListDto>();
        var waitingList = _waitingListRepository.GetWaitingList(name, false);
        result.Messages = waitingList.Messages;
        var waitingListDto = GetDto(waitingList.Records.FirstOrDefault());
        if (waitingListDto != null)
        {
            result.Records.Add(waitingListDto);
        }
        return result;
    }

    private WaitingListDto? GetDto(WaitingListEntity? waitingList)
    {
        if (waitingList == null)
        {
            return null;  
        }

        var waitingListDto = new WaitingListDto();
        waitingListDto.Name = waitingList.Name;
        waitingListDto.TotalSeats = waitingList.TotalSeats;
        waitingListDto.Parties = waitingList.Parties.ToDto();
        waitingListDto.Id = waitingList.Id;
        waitingListDto.SeatsAvailable = CalculateSeatsAvailable(waitingList);
        return waitingListDto;   
    }


    private bool WaitingListHasSeatCapacity(WaitingListEntity waitingList, int amountOfSeatsNeeded)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt == null && !p.CheckedIn);
        var amountOfSeatsTaken = parties.Sum((p) => p.Size);  
        return amountOfSeatsTaken + amountOfSeatsNeeded <= waitingList.TotalSeats;;
    }

    private int CalculateSeatsAvailable(WaitingListEntity waitingList)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt == null && !p.CheckedIn);
        var currentlySeatedAmount =  parties.Sum((p) => p.Size);  
        return waitingList.TotalSeats - currentlySeatedAmount;   
    }
}