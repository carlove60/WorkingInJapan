using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

/// <summary>
/// Represents the response returned for waiting list-related API operations.
/// This response contains the waiting list details along with any validation messages.
/// </summary>
public class WaitingListResponse : BaseResponse
{
    /// <summary>
    /// Gets or sets the waiting list details, which include information about the
    /// parties, total seats, available seats, and the name of the waiting list.
    /// </summary>
    public WaitingListDto WaitingList { get; set; }
}