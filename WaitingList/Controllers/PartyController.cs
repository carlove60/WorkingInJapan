using Microsoft.AspNetCore.Mvc;
using WaitingList.Entities;
using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;

namespace WaitingList.Controllers;

/// <summary>
/// Controller responsible for managing operations related to the waiting list.
/// Provides endpoints for retrieving and managing waiting list data.
/// </summary>
[ApiController]
[Route("api/party")]
public class PartyController : ControllerBase
{
    private readonly IWaitingListService _waitingListService;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitingListService"></param>
    public PartyController(IWaitingListService waitingListService)
    {
        this._waitingListService = waitingListService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="party"></param>
    /// <returns></returns>
    [Route("check-in")]
    [HttpPost]
    public ActionResult<ResultObject<PartyEntity>> CheckIn(CheckInRequest party)
    {

        return Ok(_waitingListService.CheckIn(party));
    }
}