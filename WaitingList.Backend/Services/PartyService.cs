using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Enums;
using WaitingListBackend.Extensions;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Services;

/// <summary>
/// Provides services for managing party-related operations including check-in, cancellation, retrieval,
/// and determining eligibility for check-in.
/// </summary>
/// <remarks>
/// This service interacts with <c>IPartyRepository</c> for managing party data,
/// <c>IWaitingListRepository</c> for waiting list operations, and
/// <c>SseChannelManager</c> for server-sent events communication.
/// </remarks>
public class PartyService : IPartyService
{
    /// <summary>
    /// Represents the repository abstraction for party-related data access operations.
    /// This variable allows the PartyService to interact with the data layer for performing
    /// operations such as retrieving, saving, or removing party entities.
    /// </summary>
    private readonly IPartyRepository _partyRepository;

    /// <summary>
    /// Represents the repository abstraction for waiting list-related data access operations.
    /// This variable facilitates interaction with the data layer, enabling operations such as
    /// retrieving and managing waiting list entities within the PartyService.
    /// </summary>
    private readonly IWaitingListRepository _waitingListRepository;

    /// <summary>
    /// Represents the manager responsible for handling server-sent events (SSE) communication.
    /// This variable is used to manage the lifecycle of SSE channels, send targeted messages
    /// to specific clients, or broadcast information to all connected clients.
    /// </summary>
    private readonly SseChannelManager _sseChannelManager;

    /// <summary>
    /// Provides services for managing party-related operations including check-in, cancellation, retrieval,
    /// and determining eligibility for check-in.
    /// </summary>
    /// <remarks>
    /// This service interacts with <see cref="IPartyRepository"/> for managing party data,
    /// <see cref="IWaitingListRepository"/> for waiting list operations, and
    /// <see cref="SseChannelManager"/> for server-sent events communication.
    /// </remarks>
    public PartyService(IPartyRepository partyRepository, IWaitingListRepository waitingListRepository,
        SseChannelManager sseChannelManager)
    {
        _partyRepository = partyRepository;
        _waitingListRepository = waitingListRepository;
        _sseChannelManager = sseChannelManager;
    }

    /// <summary>
    /// Checks in the party associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The session ID associated with the party to check in.</param>
    /// <returns>A <see cref="ResultObject{PartyDto}"/> containing the result of the check-in operation, including success or error messages and the updated party data.</returns>
    public ResultObject<PartyDto> CheckIn(string sessionId)
    {
        var result = new ResultObject<PartyDto>(); 
        var partyResult = _partyRepository.GetParty(sessionId); 
        result.Messages.AddRange(partyResult.Messages);
        if (result.Messages.Count > 0)
        {
            return result;
        }

        if (partyResult.Records.Count == 1 && partyResult.Records.First().CheckedIn)
        {
            result.Records.Add(partyResult.Records.First().ToDto());
            return result;
        }

        var party = partyResult.Records.First(); 
        var partyCanCheckInResult = CanCheckIn(party.SessionId);
        result.Messages.AddRange(partyCanCheckInResult.Messages);
        if (result.Messages.Count > 0 || !partyCanCheckInResult.Records.Single())
        {
            return result;
        }

        party.ServiceStartedAt = DateTime.Now;
        party.CheckedIn = true;
        _partyRepository.SaveParty(party); 
        var partyDto = party.ToDto();
        result.Records.Add(partyDto); 
        result.Messages.AddSuccess("Party checked in successfully."); 
        return result;
    }

    /// <summary>
    /// Cancels the check-in for a party associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The session ID associated with the party to cancel check-in for.</param>
    /// <returns>A <see cref="ResultObject{PartyDto}"/> containing the result of the operation, including any error messages or the affected party data.</returns>
    public ResultObject<PartyDto> CancelCheckIn(string sessionId)
    {
        var result = new ResultObject<PartyDto>();
        if (String.IsNullOrWhiteSpace(sessionId))
        {
            result.Messages.AddError("No session id found for your sign-up");
            return result;
        }
        var partyResult = _partyRepository.GetParty(sessionId);
        result.Messages.AddRange(partyResult.Messages);
        if (partyResult.Messages.Count > 0)
        {
            return result;
        }

        var party = partyResult.Records.Single();
        var deleteResult = _partyRepository.RemoveParty(party);
        result.Messages.AddRange(deleteResult.Messages);
        var partyDto = party.ToDto();
        result.Records.Add(partyDto);
        var hasErrorMessages = result.Messages.Count((x) => x.Type == MessageType.Error) > 0;
        if (hasErrorMessages)
        {
            return result;
        }
        var waitingList = _waitingListRepository.GetWaitingList(partyDto.WaitingListName, false).Records.First();
        MessageWaitingList(waitingList.ToDto());
        NotifyNextPartyToCheckIn(waitingList);
        return result;
    }


    /// <summary>
    /// Determines whether the party associated with the given session ID can be checked in at this time.
    /// </summary>
    /// <param name="sessionId">The session ID of the party to evaluate for check-in eligibility.</param>
    /// <returns>A <see cref="ResultObject{Boolean}"/> indicating if the party can be checked in and including any relevant error messages.</returns>
    public ResultObject<bool> CanCheckIn(string sessionId)
    {
        var result = new ResultObject<bool>();
        var partyEntityResult = _partyRepository.GetParty(sessionId);
        result.Messages.AddRange(partyEntityResult.Messages);
        if (result.Messages.Count > 0)
        {
            result.Records.Add(false);
            return result;
        }
        var party = partyEntityResult.Records.Single();
        var waitingListResult = _waitingListRepository.GetWaitingListWithAllParties(party.WaitingListId);
        result.Messages.AddRange(waitingListResult.Messages);
        if (waitingListResult.Records.Count == 0)
        {
            result.Records.Add(false);
            return result;       
        }
        
        var waitingList = waitingListResult.Records.Single();
        var nextPartyToCheckIn = GetNextPartyToCheckIn(waitingList.Parties);
        var partiesInService = waitingList.Parties
            .Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);

        var timeStillInService = CalculateRemainingServiceTime(partiesInService, waitingList.TimeForService);
        if (timeStillInService == 0 && nextPartyToCheckIn != null && nextPartyToCheckIn.SessionId == sessionId && PartySizeFits(waitingList, nextPartyToCheckIn.Size))
        {
            result.Records.Add(true);
        } 
        else
        {
            result.Records.Add(false);
            result.Messages.AddError("Party cannot be checked in at this time.");
        }
        return result;
    }

    /// <summary>
    /// Retrieves the party details associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The session ID used to identify the party to retrieve.</param>
    /// <returns>A <see cref="ResultObject{PartyDto}"/> containing the party details and any relevant messages.</returns>
    public ResultObject<PartyDto> GetParty(string sessionId)
    {
        var result = new ResultObject<PartyDto>();
        var party = _partyRepository.GetParty(sessionId);
        result.Messages = party.Messages;
        if (party.Records.Count == 0)
        {
            return result;
        }
        
        var partyEntity = party.Records.First();
        var canCheckInResult = CanCheckIn(partyEntity.SessionId);
        result.Messages.AddRange(canCheckInResult.Messages);
        var partyDto = partyEntity.ToDto();
        partyDto.IsServiceDone = partyEntity.ServiceEndedAt != null;
        if (!partyDto.IsServiceDone)
        {
            partyDto.CanCheckIn = canCheckInResult.Records.First();
        }

        result.Records.Add(partyDto);
        return result;
    }
    
    /// <summary>
    /// Determines the next party to check in from a collection of parties.
    /// </summary>
    /// <param name="parties">A collection of party entities to evaluate for the next check-in.</param>
    /// <returns>The next <see cref="PartyEntity"/> to check in based on the earliest creation time, or null if no eligible party exists.</returns>
    public PartyEntity? GetNextPartyToCheckIn(IEnumerable<PartyEntity> parties)
    {
        var nextPartyToCheckIn = parties.Where((p) => !p.CheckedIn)
            .OrderBy((p) => p.CreatedOn)
            .FirstOrDefault();
         return nextPartyToCheckIn;
    }
    
    /// <summary>
    /// Calculates the total remaining service time for the parties currently in service.
    /// </summary>
    /// <param name="partiesInService">A collection of parties currently in service.</param>
    /// <param name="timeForService">The standard time allocated for servicing each unit of party size.</param>
    /// <returns>The total remaining service time for all parties in service, expressed as an integer.</returns>
    private int CalculateRemainingServiceTime(IEnumerable<PartyEntity> partiesInService, int timeForService)
    {
        return partiesInService.Sum((p) => p.Size) * timeForService;
    }

    /// <summary>
    /// Determines whether the size of a party can fit in the available seats of the specified waiting list.
    /// </summary>
    /// <param name="waitingList">The waiting list entity containing the available seats and party information.</param>
    /// <param name="partySize">The size of the party to be checked against the available seats.</param>
    /// <returns>True if the party size can fit within the available seats; otherwise, false.</returns>
    private bool PartySizeFits(WaitingListEntity waitingList, int partySize)
    {
        var waitingListDto = waitingList.ToDto();
        return waitingListDto.SeatsAvailable >= partySize;
    }

    /// <summary>
    /// Notifies the next party in the specified waiting list that they are eligible to check in.
    /// </summary>
    /// <param name="waitingList">The waiting list containing the parties to check in from.</param>
    private void NotifyNextPartyToCheckIn(WaitingListEntity waitingList)
    {
        var nextPartyToCheckIn = GetNextPartyToCheckIn(waitingList.Parties);
        if (nextPartyToCheckIn == null)
        {
            return;
        }

        var canCheckIn = CanCheckIn(nextPartyToCheckIn.SessionId).Records.First();
        if (canCheckIn)
        {
            var partyDto = nextPartyToCheckIn.ToDto();
            partyDto.CanCheckIn = canCheckIn;
            MessageParty(partyDto);;
        }
    }

    /// <summary>
    /// Sends a message containing the specified party data to the associated client through the server-sent events (SSE) channel.
    /// </summary>
    /// <param name="party">The <see cref="PartyDto"/> containing the party information to be sent via the SSE channel.</param>
    private void MessageParty(PartyDto party)
    {
        _sseChannelManager.SendDto(party.SessionId, new SseDto<PartyDto>(party, nameof(PartyDto)));
    }

    /// <summary>
    /// Sends the updated waiting list details to all active clients using Server-Sent Events (SSE).
    /// </summary>
    /// <param name="waitingList">The updated waiting list information to broadcast, represented as a <see cref="WaitingListDto"/>.</param>
    private void MessageWaitingList(WaitingListDto waitingList)
    {
        _sseChannelManager.BroadcastDto(new SseDto<WaitingListDto>(waitingList, nameof(WaitingListDto)));
    }
}