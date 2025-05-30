using WaitingList.Models;

namespace WaitingList.Responses;

public class CheckInResponse : BaseResponse
{
    public bool Success { get; set; }
    
    public string Name { get; set; }
}