using WaitingList.Models;

namespace WaitingList.Responses;

public class AddToQueueResponse : BaseResponse<WaitingListModel>
{
    public string Message { get; set; }
}