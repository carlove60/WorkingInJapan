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
    /// Represents the service responsible for managing waiting list operations,
    /// such as adding parties to the waiting list and retrieving waiting list details.
    /// This variable facilitates the integration of waiting list functionality within the PartyService.
    /// </summary>
    private readonly IWaitingListService _waitingListService;

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
        SseChannelManager sseChannelManager, IWaitingListService waitingListService)
    {
        _partyRepository = partyRepository;
        _waitingListRepository = waitingListRepository;
        _sseChannelManager = sseChannelManager;
        _waitingListService = waitingListService;
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
        var partyCanCheckInResult = _waitingListService.CanCheckIn(party.SessionId);
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
        var waitingList = _waitingListService.GetWaitingList(partyDto.WaitingListName).Records.First();
        MessageWaitingList(waitingList);
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
        var waitingList = _waitingListService.GetWaitingList(partyDto.WaitingListName).Records.First();
        MessageWaitingList(waitingList);
        NotifyNextPartyToCheckIn(waitingList.NextPartyToCheckIn);
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
        var canCheckInResult = _waitingListService.CanCheckIn(partyEntity.SessionId);
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
    /// Notifies the next party in the queue, if applicable, about their eligibility to check in.
    /// </summary>
    /// <param name="party">The party details to notify, including information such as session ID, waiting list name, and check-in status. If null, no notification will be sent.</param>
    private void NotifyNextPartyToCheckIn(PartyDto? party)
    {
        if (party == null)
        {
            return;
        }

        MessageParty(party);
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