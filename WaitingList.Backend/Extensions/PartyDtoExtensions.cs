using WaitingList.DTO;
using WaitingListBackend.Entities;

namespace WaitingListBackend.Extensions;

public static class PartyDtoExtensions
{
    public static PartyEntity ToEntity(this PartyDto dto)
    {
        return new PartyEntity { Name = dto.Name, Size = dto.Size, SessionId = dto.SessionId };
    }
}