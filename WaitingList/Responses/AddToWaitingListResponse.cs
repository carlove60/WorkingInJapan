using WaitingList.Entities;
using WaitingList.Models;

namespace WaitingList.Responses;

public class AddToWaitingListResponse : BaseResponse
{
    public string PartyName { get; set; }
    
    public List<PartyEntity> Parties { get; set; } = new List<PartyEntity>();
}