
using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

/// <summary>
/// Represents the response returned when a party checks in to the waiting list.
/// This response includes details about the checked-in party as well as any
/// validation messages from the operation.
/// </summary>
public class CheckInResponse : BaseResponse
{
    /// <summary>
    /// Gets or sets the details of the party that has checked in.
    /// Provides information such as the party's waiting list name, session identifier,
    /// party name, size, and check-in status.
    /// </summary>
    public PartyDto Party { get; set; }
}