using Eeckhoven.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eeckhoven;

public class ResultObject<T>
{
    public List<ActionResult<T>> Records { get; set; } = new List<ActionResult<T>>();
    
    public MessageList UserMessages { get; set; } = new MessageList();
    public List<string> SystemMessages { get; set; } = new List<string>();
    public bool IsError { get; set; } = true;
    
    public int HttpErrorCode { get; set; }
    
    public string HttpErrorMessage { get; set; }

    public void Ok(T value)
    {
        HttpErrorCode = 200;
        Records.Add(value);
    }

    public void BadRequest(string message, int errorCode)
    {
        HttpErrorCode = errorCode;
        IsError = true;
        HttpErrorMessage = message;

    }

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