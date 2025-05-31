
using WaitingList.DTO;

namespace WaitingList.Responses;

public class WaitingListMetaDataResponse : BaseResponse
{
    
    public WaitingListDto WaitingList { get; set; }
    
}