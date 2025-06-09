using System.ComponentModel.DataAnnotations;
using WaitingListBackend.Enums;

namespace WaitingListBackend;

/// <summary>
/// Represents a result object that contains a list of records and associated messages.
/// </summary>
/// <typeparam name="T">The type of objects contained in the result list.</typeparam>
public class ResultObject<T>
{
    /// <summary>
    /// Gets or sets the collection of records contained within the result object.
    /// This property holds the entities or data objects relevant to the operation's output,
    /// such as entities retrieved from or affected by the operation.
    /// </summary>
    [Required]
    public List<T> Records { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of messages associated with the result object.
    /// This property is used to store validation, error, warning, or informational messages
    /// that provide additional context or feedback regarding the operation or data processing.
    /// </summary>
    [Required]
    public MessageList Messages { get; set; } = new();

    public bool HasErrors()
    {
        return Messages.Any((m) => m.Type == MessageType.Error);
    }
}