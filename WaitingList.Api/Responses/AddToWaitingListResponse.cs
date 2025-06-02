using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

/// <summary>
/// Represents the response provided after attempting to add a party to the waiting list.
/// </summary>
public class AddToWaitingListResponse : BaseResponse
{
    /// <summary>
    /// Gets or sets the party information associated with the response.
    /// This property contains details about the party that was added to the waiting list,
    /// including its name, size, session ID, and check-in status.
    /// </summary>
    public PartyDto Party { get; set; }
}