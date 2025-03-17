using Eeckhoven.Enums;

namespace Eeckhoven.Models;

public class MessageModel
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public MessageType MessageTypeType { get; set; }
}