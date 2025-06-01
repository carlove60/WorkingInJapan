using WaitingListBackend.Enums;
using WaitingListBackend.Models;

namespace WaitingListBackend;

/// <summary>
/// Represents a collection of validation messages with utility functions for adding error, warning, or informational messages.
/// Inherits from the <see cref="List{ValidationMessage}" /> class.
/// </summary>
public class MessageList : List<ValidationMessage>
{
    /// <summary>
    /// Adds an error message to the message list with a predefined message type.
    /// </summary>
    /// <param name="message">The error message to be added to the message list.</param>
    public void AddError(string message)
    {
        Add(new ValidationMessage { Type = MessageType.Error, Message = message});
    }

    /// <summary>
    /// Adds a warning message to the message list with a predefined message type.
    /// </summary>
    /// <param name="message">The warning message to be added to the message list.</param>
    public void AddWarning(string message)
    {
        Add(new ValidationMessage { Type = MessageType.Warning, Message = message});
    }

    public void AddSuccess(string message)
    {
        Add(new ValidationMessage { Type = MessageType.Success, Message = message});   
    }

    /// <summary>
    /// Adds an informational message to the message list with a predefined message type.
    /// </summary>
    /// <param name="message">The informational message to be added to the message list.</param>
    public void AddInfo(string message)
    {
        Add(new ValidationMessage { Type = MessageType.Info, Message = message});
    }
}