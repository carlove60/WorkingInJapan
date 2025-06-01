using WaitingList.Contracts.DTOs;
using WaitingList.Extensions;
using WaitingListBackend.Entities;
using WaitingListBackend.Extensions;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Services;

public class PartyService : IPartyService
{
    private readonly IPartyRepository _partyRepository;
    private readonly IWaitingListRepository _waitingListRepository;
    
    public PartyService(IPartyRepository partyRepository, IWaitingListRepository waitingListRepository)
    {
        _partyRepository = partyRepository;
        _waitingListRepository = waitingListRepository;
    }
    
    public ResultObject<PartyDto> CheckIn(string sessionId)
    { 
        var result = new ResultObject<PartyDto>(); 
        var partyResult = _partyRepository.GetParty(sessionId); 
        result.Messages.AddRange(result.Messages);
        if (partyResult.Records.Count == 0)
        {
            result.Messages.AddError("No party found for this session. Please check if you used a different browser."); 
            return result;
        }
        var party = partyResult.Records.First(); 
        var partyCanCheckInResult = CanCheckIn(party);
        result.Messages.AddRange(partyCanCheckInResult.Messages);
        if (result.Messages.Count > 0)
        {
            return result;
        }

        party.ServiceStartedAt = DateTime.Now; 
        _partyRepository.SaveParty(party); 
        result.Records.Add(party.ToDto()); 
        result.Messages.AddSuccess("Party checked in successfully."); 
   
        return result;
    }

    
    private ResultObject<PartyDto> CanCheckIn(PartyEntity partyEntity)
    {
        var result = new ResultObject<PartyDto>();
        var waitingListResult = _waitingListRepository.GetWaitingList(partyEntity.WaitingListId);
        result.Messages.AddRange(waitingListResult.Messages);
        if (!waitingListResult.Records.Any())
        {
            return result;
        }
        
        var waitingList = waitingListResult.Records.Single();
        var nextPartyToCheckIn = waitingList.Parties.Where((p) => p.ServiceStartedAt == null && p.ServiceEndedAt == null)
            .OrderBy((p) => p.CreatedOn)
            .First();

        var party = partyEntity.ToDto();
        var partiesInService = waitingList.Parties
            .Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);

        var timeStillInService = CalculateRemainingServiceTime(partiesInService, waitingList.TimeForService);
        if (timeStillInService == 0 && nextPartyToCheckIn.SessionId == party.SessionId && PartySizeFits(waitingList, nextPartyToCheckIn.Size))
        {
            party.CanCheckIn = true;
        } 
        else
        {
            party.CanCheckIn = false;
        }
        
        result.Records.Add(party);
        
        
        return result;
    }

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
        var checkedInParty = CanCheckIn(partyEntity);
        result.Messages.AddRange(checkedInParty.Messages);
        if (checkedInParty.Records.Count == 1)
        {
            result.Records.Add(checkedInParty.Records.First());
        }
        return result;
    }
    
    private int CalculateRemainingServiceTime(IEnumerable<PartyEntity> partiesInService, int timeForService)
    {
        return partiesInService.Sum((p) => p.Size) * timeForService;
    }

    private bool PartySizeFits(WaitingListEntity waitingList, int partySize)
    {
        var waitingListDto = waitingList.ToDto();
        return waitingListDto.SeatsAvailable >= partySize;
    }
}