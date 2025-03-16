using Eeckhoven.Enums;
using Eeckhoven.Models;

namespace Eeckhoven;

public class MessageList : List<ValidationMessage>
{
    public void AddError(string messageEnglish, string messageJapanese)
    {
        Add(new ValidationMessage { Type = MessageTypeEnum.Error, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }

    public void AddWarning(string messageEnglish, string messageJapanese)
    {
        Add(new ValidationMessage { Type = MessageTypeEnum.Warning, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }

    public void AddInfo(string messageEnglish, string messageJapanese) 
    {
        Add(new ValidationMessage { Type = MessageTypeEnum.Info, MessageEnglish = messageEnglish, MessageJapanese = messageJapanese});
    }
}