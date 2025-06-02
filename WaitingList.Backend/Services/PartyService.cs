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
        party.CheckedIn = true;
        _partyRepository.SaveParty(party); 
        result.Records.Add(party.ToDto()); 
        result.Messages.AddSuccess("Party checked in successfully."); 
   
        return result;
    }

    
    private ResultObject<bool> CanCheckIn(PartyEntity partyEntity)
    {
        var result = new ResultObject<bool>();
        var waitingListResult = _waitingListRepository.GetWaitingListWithAllParties(partyEntity.WaitingListId);
        result.Messages.AddRange(waitingListResult.Messages);
        if (!waitingListResult.Records.Any())
        {
            result.Records.Add(false);
            return result;       
        }
        
        var waitingList = waitingListResult.Records.Single();
        var nextPartyToCheckIn = waitingList.Parties.Where((p) => !p.CheckedIn)
            .OrderBy((p) => p.CreatedOn)
            .FirstOrDefault();

        var party = partyEntity.ToDto();
        var partiesInService = waitingList.Parties
            .Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);

        var timeStillInService = CalculateRemainingServiceTime(partiesInService, waitingList.TimeForService);
        if (timeStillInService == 0 && nextPartyToCheckIn != null && nextPartyToCheckIn.SessionId == party.SessionId && PartySizeFits(waitingList, nextPartyToCheckIn.Size))
        {
            result.Records.Add(true);
        } 
        else
        {
            result.Records.Add(false);
        }
        
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
        if (partyEntity.ServiceEndedAt != null && partyEntity.CheckedIn)
        {
            ResetParty(partyEntity);
        }

        var checkedInParty = CanCheckIn(partyEntity);
        result.Messages.AddRange(checkedInParty.Messages);
        var partyDto = partyEntity.ToDto();
        if (checkedInParty.Records.Count == 1)
        {
            partyDto.CanCheckIn = checkedInParty.Records.First();
        }
        result.Records.Add(partyDto);
        return result;
    }
    
    private void ResetParty(PartyEntity partyEntity)
    {
        _partyRepository.RemoveParty(partyEntity);
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