using System.ComponentModel.DataAnnotations;
using WaitingListBackend.Models;

namespace WaitingList.Responses;

/// <summary>
/// Represents the base response for API operations. This class serves as the foundation
/// for other response types, providing a common structure for returning validation messages.
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Gets or sets a collection of validation messages associated with the response.
    /// These messages provide feedback on the outcome of an operation, including errors,
    /// warnings, and informational messages.
    /// </summary>
    [Required]
    public List<ValidationMessage> Messages { get; set; } = new List<ValidationMessage>();
}