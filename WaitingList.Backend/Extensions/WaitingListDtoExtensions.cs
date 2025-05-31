using WaitingList.DTO;
using WaitingListBackend.Entities;

namespace WaitingList.Extensions;

/// <summary>
/// Provides extension methods for converting entities to DTOs in the context of WaitingList.Api operations.
/// </summary>
public static class WaitingListDtoExtensions
{
    /// <summary>
    /// Converts a <see cref="WaitingListEntity"/> object to a <see cref="WaitingListDto"/> object.
    /// </summary>
    /// <param name="waitingList">The <see cref="WaitingListEntity"/> to be converted.</param>
    /// <returns>A <see cref="WaitingListDto"/> object containing the converted data.</returns>
    public static WaitingListDto ToDto(this WaitingListEntity waitingList)
    {
        return new WaitingListDto { Name = waitingList.Name, Id = waitingList.Id, Parties = waitingList.Parties.ToDto(), TotalSeatsAvailable = waitingList.TotalSeats };
    }
}