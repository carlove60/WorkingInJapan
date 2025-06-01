
using WaitingList.Contracts.DTOs;

namespace WaitingList.Responses;

public class CheckInResponse : BaseResponse
{
    public PartyDto Party { get; set; }
}