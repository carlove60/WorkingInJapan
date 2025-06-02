using WaitingList.Contracts.DTOs;
using WaitingListBackend.Entities;

namespace WaitingListBackend.Extensions;

/// <summary>
/// Provides extension methods for converting PartyEntity objects to PartyDto objects.
/// </summary>
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