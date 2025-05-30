
namespace WaitingList.Requests;

public class AddToWaitingListRequest
{
    public Guid WaitingListId { get; set; }
    public string PartyName { get; set; }
    public int PartySize { get; set; }
}