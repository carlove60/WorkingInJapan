using Microsoft.AspNetCore.Mvc;
using WaitingList.Extensions;
using WaitingList.Interfaces;
using WaitingList.Models;
using WaitingList.Requests;
using WaitingList.Responses;

namespace WaitingList.Controllers;

/// <summary>
/// Controller responsible for managing operations related to the waiting list.
/// Provides endpoints for retrieving and managing waiting list data.
/// </summary>
[ApiController]
[Route("api/waitinglist")]
public class WaitingListController : ControllerBase
{
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly IWaitingListService _waitingListService;

    /// <summary>
    /// Controller responsible for handling operations on a waiting list.
    /// Provides endpoints for retrieving the current waiting list, managing metadata,
    /// and adding parties to the waiting list.
    /// </summary>
    public WaitingListController(IWaitingListRepository waitingListRepository, IWaitingListService waitingListService)
    {
        this._waitingListRepository = waitingListRepository;
        this._waitingListService = waitingListService;
    }

    /// <summary>
    /// Retrieves the current waiting list and wraps it in a response object.
    /// This method interacts with the repository to fetch the waiting list data
    /// and encapsulates the result for the API response.
    /// </summary>
    /// <returns>An action result containing a response object with the waiting list data.</returns>
    [HttpGet]
    
    [Route("waiting-list")]
    public ActionResult<WaitingListResponse> GetWaitingList([FromQuery] string waitingListName)
    {
        if (waitingListName.IsNullOrWhiteSpace())
        {   
            return BadRequest("No waiting list name provided");  
        }
        var result = _waitingListRepository.GetWaitingList(waitingListName);
        var response = new WaitingListResponse { Result = result };
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the metadata associated with the waiting list.
    ///
    /// This method interacts with the service layer to obtain and return metadata
    /// </summary>
    /// <returns>An action result containing a response object with the metadata of the waiting list.</returns>
    [HttpGet]
    [Route("waitinglist-meta-data")]
    public ActionResult<WaitingListMetaDataResponse> GetMetaData()
    {
        var response = _waitingListService.GetMetaData();
        return Ok(response);
    }


    /// <summary>
    /// Checks in a party to the waiting list based on the request provided.
    /// This action validates the input request and processes the check-in
    /// operation through the waiting list service.
    /// </summary>
    /// <param name="request">The request object containing the details required for checking in a party.</param>
    /// <returns>An action result containing a response object with the details of the checked-in party.</returns>
    [Route("check-in")]
    [HttpPost]
    public ActionResult<CheckInResponse> CheckIn(CheckInRequest? request)
    {
        if (request == null)
        {   
            return BadRequest("No request provided");  
        }
        
        return Ok(_waitingListService.CheckIn(request));
    }

    /// <summary>
    /// Adds a party to the waiting list based on the provided request.
    /// </summary>
    /// <param name="request">An object containing details about the party to be added to the waiting list.</param>
    /// <returns>A response indicating the result of adding the party to the waiting list.</returns>
    [Route("add-party-to-waitinglist")]
    [HttpPost]
    public ActionResult<WaitingListModel> AddPartyToWaitingList(AddToQueueRequest? request)
    {
        if (request == null)
        {   
            return BadRequest("No request provided");  
        }
        
        return Ok(_waitingListService.AddPartyToWaitingList(request));
    }
}