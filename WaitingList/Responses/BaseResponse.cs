using WaitingList.Models;

namespace WaitingList.Responses;

public class BaseResponse<T>
{
    public ResultObject<T> Result { get; set; } = new ResultObject<T>();
}