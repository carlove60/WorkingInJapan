using WaitingList.DTO;
using WaitingListBackend.Entities;

namespace WaitingList.Extensions;

public static class PartyEntityExtensions
{
    public static List<PartyDto> ToDto(this List<PartyEntity> parties)
    {
        return parties.Select((partyEntity) => new PartyDto { Name = partyEntity.Name, Id = partyEntity.Id, Size = partyEntity.Size, WaitingListId = partyEntity.WaitingListId}).ToList();
    }
}