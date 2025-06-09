using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Extensions;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Services;

/// <summary>
/// Service responsible for handling operations related to the waiting list functionality in the application.
/// </summary>
public class WaitingListService : IWaitingListService
{
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IPartyService _partyService;
    private readonly SseChannelManager _sseChannelManager;

    /// <summary>
    /// Provides services for managing and interacting with waiting lists.
    /// </summary>
    public WaitingListService(IWaitingListRepository waitingListRepository, IPartyRepository partyRepository,
        IPartyService partyService, SseChannelManager sseChannelManager)
    {
        _waitingListRepository = waitingListRepository;
        _partyRepository = partyRepository;
        _partyService = partyService;   
        _sseChannelManager = sseChannelManager;  
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
        lock (_waitingListRepository)
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
            
            var hasSeatCapacity = WaitingListHasSeatCapacity(waitingList, partyDto.Size);
            if (!hasSeatCapacity)
            {
                result.Messages.AddError(
                    $"There are only {CalculateSeatsAvailable(waitingList)} seat(s) left on the waiting list. Please when there are more seats available, you will be sent a notification on this page.");
                return result;
            }

            var partyEntity = partyDto.ToEntity();
            partyEntity.WaitingListId = waitingList.Id;
            var party = _partyRepository.SaveParty(partyEntity);
            partyDto.CanCheckIn = _partyService.CanCheckIn(partyDto.SessionId).Records.First();;
            waitingListDto.AddedParty = partyDto;
            result.Records.Add(waitingListDto);
            if (party.Messages.Count > 0)
            {
                result.Messages.AddRange(party.Messages);
                return result;       
            }
            
            waitingListDto = GetWaitingList(waitingList.Name).Records.First();
            var sseWaitingListDto = new SseDto<WaitingListDto>(waitingListDto, nameof(WaitingListDto));
            _sseChannelManager.BroadcastDto(sseWaitingListDto);
            result.Messages.AddSuccess("You have been successfully added to the waiting list!");
        }
        
        return result;
    }

    /// <summary>
    /// Retrieves a waiting list by its name.
    /// </summary>
    /// <param name="name">The name of the waiting list to retrieve.</param>
    /// <returns>A result object containing the information of the requested waiting list as a <see cref="WaitingListDto"/> and any relevant messages.</returns>
    public ResultObject<WaitingListDto> GetWaitingList(string name)
    {
        var result = new ResultObject<WaitingListDto>();
        var waitingList = _waitingListRepository.GetWaitingList(name, false);
        result.Messages = waitingList.Messages;
        var waitingListDto = CreateDto(waitingList.Records.FirstOrDefault());
        if (waitingListDto != null)
        {
            result.Records.Add(waitingListDto);
        }
        return result;
    }

    /// <summary>
    /// Converts a <see cref="WaitingListEntity"/> object into a <see cref="WaitingListDto"/> object.
    /// </summary>
    /// <param name="waitingList">
    /// The <see cref="WaitingListEntity"/> instance to be converted. If null, the method returns null.
    /// </param>
    /// <returns>
    /// A <see cref="WaitingListDto"/> object containing the data from the given <see cref="WaitingListEntity"/>.
    /// Returns null if the input <see cref="WaitingListEntity"/> is null.
    /// </returns>
    private WaitingListDto? CreateDto(WaitingListEntity? waitingList)
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

    /// <summary>
    /// Determines if a waiting list has enough available seat capacity to accommodate a specified number of seats.
    /// </summary>
    /// <param name="waitingList">The waiting list entity that contains the parties and the total seat count.</param>
    /// <param name="amountOfSeatsNeeded">The number of seats required.</param>
    /// <returns>True if the waiting list has enough available capacity; otherwise, false.</returns>
    private bool WaitingListHasSeatCapacity(WaitingListEntity waitingList, int amountOfSeatsNeeded)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt == null && !p.CheckedIn);
        var amountOfSeatsTaken = parties.Sum((p) => p.Size);  
        return amountOfSeatsTaken + amountOfSeatsNeeded <= waitingList.TotalSeats;;
    }

    /// <summary>
    /// Calculates the number of seats available on the waiting list by subtracting
    /// the total size of active unseated parties from the total seats of the waiting list.
    /// </summary>
    /// <param name="waitingList">The waiting list entity containing parties and total seats.</param>
    /// <returns>The number of seats currently available.</returns>
    private int CalculateSeatsAvailable(WaitingListEntity waitingList)
    {
        var parties = waitingList.Parties.Where((p) => p.ServiceEndedAt == null && !p.CheckedIn);
        var currentlySeatedAmount =  parties.Sum((p) => p.Size);  
        return waitingList.TotalSeats - currentlySeatedAmount;   
    }
}