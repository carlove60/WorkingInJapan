using WaitingList.Models;

namespace WaitingList.Requests;

public class AddToQueueRequest
{
    public Guid WaitingListId { get; set; }
    public PartyModel Party { get; set; }
}