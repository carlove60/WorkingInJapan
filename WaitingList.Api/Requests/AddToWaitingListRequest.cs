
using WaitingList.Contracts.DTOs;

namespace WaitingList.Requests;

public class AddToWaitingListRequest
{
    public PartyDto Party { get; set; }
}