using WaitingList.DTO;

namespace WaitingList.Responses;

public class WaitingListResponse : BaseResponse
{
    public List<PartyDto> Parties { get; set; } = new List<PartyDto>();
}