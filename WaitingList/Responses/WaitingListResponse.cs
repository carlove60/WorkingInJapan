using WaitingList.Entities;
using WaitingList.Models;

namespace WaitingList.Responses;

public class WaitingListResponse : BaseResponse
{
    public List<PartyEntity> Parties { get; set; } = new List<PartyEntity>();
}