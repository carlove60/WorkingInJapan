using Eeckhoven.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eeckhoven;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResultObject<T>
{
    public ResultObject(string httpErrorMessage)
    {
        HttpErrorMessage = httpErrorMessage;
    }

    public ResultObject()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public List<ActionResult<T>> Records { get; set; } = new List<ActionResult<T>>();
    
    public MessageList UserMessages { get; set; } = new MessageList();
    
    /// <summary>
    /// 
    /// </summary>
    public List<string> SystemMessages { get; set; } = new List<string>();
    /// <summary>
    /// 
    /// </summary>
    public bool IsError { get; set; } = true;
    
    /// <summary>
    /// 
    /// </summary>
    public int HttpErrorCode { get; set; }
    
    public string HttpErrorMessage { get; set; }
    
    public void BadRequest(string message, int errorCode)
    {
        HttpErrorCode = errorCode;
        IsError = true;
        HttpErrorMessage = message;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    public void AddResult(ActionResult<T> model)
    {
        this.Records.Add(model);
    }
    
    public void AddResult(T model, bool isError)
    {
        this.Records.Add(model);
        IsError = isError;
    }
}