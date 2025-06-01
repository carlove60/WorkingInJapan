using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

public class AddToWaitingListResponse : BaseResponse
{
    public string PartyName { get; set; }

    public PartyDto Party { get; set; }
}