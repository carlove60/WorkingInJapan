
using Eeckhoven.Enums;

namespace Eeckhoven.Models;

public class ValidationMessage
{
    public string MessageEnglish { get; set; } = "";
    public string MessageJapanese { get; set; } = "";
    public MessageTypeEnum Type { get; set; } = MessageTypeEnum.Info;
}