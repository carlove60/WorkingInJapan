using WaitingList.Contracts.DTOs;
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
        var result = new ResultObject<WaitingListDto>();
        
        // Ensure that multiple parties don't try to make a request at the same time and both reserve (a) seat(s)
        lock (this._waitingListRepository)
        {
            var waitingListResult =  _waitingListRepository.GetWaitingList(partyDto.WaitingListName, false);
            result.Messages.AddRange(waitingListResult.Messages);
            if (waitingListResult.Records.Count == 0)
            {
                return result;
            }

            var waitingList = waitingListResult.Records.First();
            
            var existingParty = waitingList.Parties.FirstOrDefault((p) => p.SessionId == partyDto.SessionId && p.ServiceStartedAt == null);
            var waitingListDto = waitingList.ToDto();
            if (existingParty != null)
            {
                waitingListDto.AddedParty = existingParty.ToDto();
                result.Records.Add(waitingListDto);
                return result;  
            }
            
            waitingListDto.AddedParty = partyDto;
            result.Records.Add(waitingListDto);
            
            var hasSeatCapacity = WaitingListHasSeatCapacity(waitingList, partyDto.Size);
            if (!hasSeatCapacity)
            {
                result.Messages.AddError(
                    $"There are only {CalculateSeatsAvailable(waitingList)} seat(s) left on the waiting list. Please when there are more seats available, you will be sent a notification on this page.");
                return result;
            }

            var partyEntity = partyDto.ToEntity();
            partyEntity.WaitingListId = waitingList.Id;
            result.Messages.AddSuccess("You have been successfully added to the waiting list!");
        
            var party = _partyRepository.SaveParty(partyEntity);
            if (party.Messages.Count > 0)
            {
                result.Messages.AddRange(party.Messages);
                return result;       
            }
        }
        
        return result;
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