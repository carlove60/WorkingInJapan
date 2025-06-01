using WaitingList.Contracts.DTOs;
using WaitingListBackend.Entities;

namespace WaitingListBackend.Extensions;

public static class PartyEntityExtensions
{
    public static List<PartyDto> ToDto(this List<PartyEntity> parties)
    {
        return parties.Select((partyEntity) => partyEntity.ToDto()).ToList();
    }
    
    public static PartyDto ToDto(this PartyEntity partyEntity)
    {
        return new PartyDto { CheckedIn = partyEntity.CheckedIn, Name = partyEntity.Name, Size = partyEntity.Size, WaitingListName = partyEntity.WaitingListEntity?.Name, SessionId = partyEntity.SessionId};
    }
}