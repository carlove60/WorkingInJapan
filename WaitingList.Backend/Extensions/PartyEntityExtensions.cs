using WaitingList.DTO;
using WaitingListBackend.Entities;

namespace WaitingList.Extensions;

public static class PartyEntityExtensions
{
    public static List<PartyDto> ToDto(this List<PartyEntity> parties)
    {
        return parties.Select((partyEntity) => new PartyDto { Name = partyEntity.Name, Size = partyEntity.Size, WaitingListName = partyEntity.WaitingListEntity.Name}).ToList();
    }
    
    public static PartyDto ToDto(this PartyEntity partyEntity)
    {
        return new PartyDto { Name = partyEntity.Name, Size = partyEntity.Size, WaitingListName = partyEntity.WaitingListEntity.Name};
    }
}