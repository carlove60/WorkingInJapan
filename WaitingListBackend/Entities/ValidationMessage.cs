using WaitingListBackend.Enums;

namespace WaitingListBackend.Entities;

/// <summary>
/// Represents a validation message that contains a text message and its associated type.
/// </summary>
public class ValidationMessage
{
    /// <summary>
    /// Represents the content of the validation message.
    /// </summary>
    public required string Message { get; set; } = "";


    /// <summary>
    /// Gets or sets the type of the validation message, which defines the category or severity level of the message.
    /// </summary>
    /// <remarks>
    /// The type is represented by the <c>MessageType</c> enumeration. Possible values include:
    /// - success
    /// - info
    /// - warning
    /// - error
    /// </remarks>
    public required MessageType Type { get; set; } = MessageType.info;
}