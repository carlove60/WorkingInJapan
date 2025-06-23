using WaitingList.Contracts.DTOs;
using WaitingList.Database.Entities;

namespace WaitingListBackend.Extensions;

/// <summary>
/// Provides extension methods for converting PartyEntity objects to PartyDto objects.
/// </summary>
public static class PartyEntityExtensions
{
    /// <summary>
    /// Converts a list of <see cref="PartyEntity"/> instances to a list of <see cref="PartyDto"/> instances.
    /// </summary>
    /// <param name="parties">The list of <see cref="PartyEntity"/> objects to be converted.</param>
    /// <returns>A list of <see cref="PartyDto"/> objects containing the mapped properties from the <paramref name="parties"/>.</returns>
    public static List<PartyDto> ToDto(this List<PartyEntity> parties)
    {
        return parties.Select((partyEntity) => partyEntity.ToDto()).ToList();
    }

    /// <summary>
    /// Converts a <see cref="PartyEntity"/> instance to a <see cref="PartyDto"/> instance.
    /// </summary>
    /// <param name="partyEntity">The <see cref="PartyEntity"/> to be converted.</param>
    /// <returns>A <see cref="PartyDto"/> instance containing the mapped properties from the <paramref name="partyEntity"/>.</returns>
    public static PartyDto ToDto(this PartyEntity partyEntity)
    {
        return new PartyDto
        {
            CheckedIn = partyEntity.CheckedIn, 
            IsServiceDone = partyEntity.ServiceEndedAt != null, 
            Name = partyEntity.Name, Size = partyEntity.Size,
            WaitingListName = partyEntity.WaitingListEntity?.Name ?? "", 
            SessionId = partyEntity.SessionId};
    }
}