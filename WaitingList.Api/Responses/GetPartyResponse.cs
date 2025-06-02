using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

/// <summary>
/// Represents a response object containing information about a specific party,
/// including its details and any validation or informational messages.
/// Inherits from <see cref="BaseResponse"/>.
/// </summary>
public class GetPartyResponse : BaseResponse
{
    /// <summary>
    /// Gets or sets the details of the party. This property contains information
    /// about a specific party such as its name, size, waiting list details, session ID,
    /// and check-in status. The data is encapsulated in the <see cref="PartyDto"/> class.
    /// </summary>
    public PartyDto Party { get; set; }
}