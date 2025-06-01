using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

public class WaitingListResponse : BaseResponse
{
    public WaitingListDto WaitingList { get; set; }
}