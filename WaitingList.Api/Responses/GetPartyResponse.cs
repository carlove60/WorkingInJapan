using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

public class GetPartyResponse : BaseResponse
{
    public PartyDto Party { get; set; }
}