using Microsoft.AspNetCore.Mvc;
using WaitingList.Requests;
using WaitingList.Responses;
using WaitingListBackend.Entities;
using WaitingListBackend.Interfaces;

namespace WaitingList.Controllers;

/// <summary>
/// Controller responsible for managing operations related to the waiting list.
/// Provides endpoints for retrieving and managing waiting list data.
/// </summary>
[ApiController]
[Route("api/waitinglist")]
public class WaitingListController : ControllerBase
{
    private readonly IWaitingListService _waitingListService;

    /// <summary>
    /// Controller responsible for handling operations on a waiting list.
    /// Provides endpoints for retrieving the current waiting list, managing metadata,
    /// and adding parties to the waiting list.
    /// </summary>
    public WaitingListController(IWaitingListService waitingListService)
    {
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
    public ActionResult<WaitingListResponse> GetWaitingList([FromQuery] Guid? id)
    {
        if (id == null || id == Guid.Empty)
        {   
            return BadRequest("No waiting list name provided");  
        }
        var result = _waitingListService.GetMetaData();
        var response = new WaitingListResponse  { Messages = result.Messages,};
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
        
        return Ok(_waitingListService.CheckIn(request.WaitingListId, request.PartyId));
    }

    /// <summary>
    /// Adds a party to the waiting list based on the provided request.
    /// </summary>
    /// <param name="request">An object containing details about the party to be added to the waiting list.</param>
    /// <returns>A response indicating the result of adding the party to the waiting list.</returns>
    [Route("add-party-to-waitinglist")]
    [HttpPost]
    public ActionResult<AddToWaitingListResponse> AddPartyToWaitingList(AddToWaitingListRequest? request)
    {
        if (request == null)
        {   
            return BadRequest("No request provided");  
        }

        var partyEntity = new PartyEntity { Name = request.PartyName, Size = request.PartySize};
        return Ok(_waitingListService.AddPartyToWaitingList(partyEntity, request.WaitingListId));
    }
}