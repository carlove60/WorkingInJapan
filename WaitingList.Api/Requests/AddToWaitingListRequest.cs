
using WaitingList.Contracts.DTOs;

namespace WaitingList.Requests;

/// <summary>
/// Represents a request to add a party to the waiting list.
/// </summary>
public class AddToWaitingListRequest
{
    /// <summary>
    /// Gets or sets the party information associated with a request to add to the waiting list.
    /// </summary>
    /// <remarks>
    /// This property represents the details of the party being added, including information
    /// such as the party name, size, and associated session ID. The property should be provided
    /// when making a request to add a party to the waiting list.
    /// </remarks>
    public PartyDto Party { get; set; }
}