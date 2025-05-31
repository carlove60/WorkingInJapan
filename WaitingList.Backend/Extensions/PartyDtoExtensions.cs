using WaitingList.DTO;
using WaitingListBackend.Entities;

namespace WaitingList.Extensions;

public static class PartyDtoExtensions
{
    public static PartyEntity ToEntity(this PartyDto dto)
    {
        return new PartyEntity { Name = dto.Name, Size = dto.Size, Id = dto.Id, WaitingListId = dto.WaitingListId };
    }
}