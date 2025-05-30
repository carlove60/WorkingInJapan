
namespace WaitingList.Responses;

public class WaitingListMetaDataResponse : BaseResponse
{
    public string WaitingListName { get; set; }
    public int TotalSeatsAvailable { get; set; }
}