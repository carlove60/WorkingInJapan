using Eeckhoven.Models;

namespace Eeckhoven;

public class ResultObject<T>
{
    public List<T> Records { get; set; } = new List<T>();
    public MessageList Messages { get; set; } = new MessageList();
    public bool IsError { get; set; } = true;
}