using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;

namespace WaitingListBackend.Extensions;

/// <summary>
/// Provides extension methods for converting PartyDto objects to PartyEntity objects
/// for use within the waiting list system.
/// </summary>
public static class PartyDtoExtensions
{
    public static PartyEntity ToEntity(this PartyDto dto)
    {
        return new PartyEntity { Name = dto.Name, Size = dto.Size, SessionId = dto.SessionId, CheckedIn = dto.CheckedIn};
    }
}