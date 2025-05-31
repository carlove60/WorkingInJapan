using WaitingList.DTO;

namespace WaitingList.Responses;

public class GetPartyResponse : BaseResponse
{
    public PartyDto Party { get; set; }
}