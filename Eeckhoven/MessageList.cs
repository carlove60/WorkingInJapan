using Eeckhoven.Enums;
using Eeckhoven.Models;

namespace Eeckhoven;

public class MessageList : List<ValidationMessage>
{
    public void AddError(string messageEnglish, string messageJapanese)
    {
        Add(new ValidationMessage { Type = MessageType.Error, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }

    public void AddWarning(string messageEnglish, string messageJapanese)
    {
        Add(new ValidationMessage { Type = MessageType.Warning, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }

    public void AddInfo(string messageEnglish, string messageJapanese) 
    {
        Add(new ValidationMessage { Type = MessageType.Info, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }
}