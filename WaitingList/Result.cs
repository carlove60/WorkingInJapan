using System.ComponentModel.DataAnnotations;
namespace WaitingList;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    [Required]    
    public List<T> Records { get; set; } = new List<T>();
    
    [Required]    
    public MessageList Messages { get; set; } = new MessageList();
    
    public bool IsError { get; set; }
}