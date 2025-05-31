using WaitingList.DTO;
using WaitingList.Extensions;
using WaitingListBackend.Interfaces;

namespace WaitingListBackend.Services;

public class PartyService : IPartyService
{
    private readonly IPartyRepository _partyRepository;
    
    public PartyService(IPartyRepository partyRepository)
    {
        _partyRepository = partyRepository;
    }

    public ResultObject<PartyDto> GetParty(Guid sessionId)
    {
        var result = new ResultObject<PartyDto>();
        var party = _partyRepository.GetParty(sessionId);
        result.Messages = party.Messages;
        if (party.Records.Count == 0)
        {
            return result;
        }
        
        var partyEntity = party.Records.First();
        result.Records.Add(partyEntity.ToDto());
        return result;
    }
}