using System.ComponentModel.DataAnnotations;
using WaitingListBackend.Models;

namespace WaitingList.Responses;

public class BaseResponse
{
    [Required]    
    public List<ValidationMessage> Messages { get; set; } = new List<ValidationMessage>();
}