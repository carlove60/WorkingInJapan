using System.ComponentModel.DataAnnotations;
using WaitingList.Models;

namespace WaitingList.Responses;

public class BaseResponse
{
    [Required]    
    public List<ValidationMessage> Messages { get; set; } = new List<ValidationMessage>();
}