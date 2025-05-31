using Microsoft.AspNetCore.Mvc;
using WaitingList.Extensions;
using WaitingList.Requests;
using WaitingList.Responses;
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

    [HttpGet]
    [Route("/waiting-list")]
    public ActionResult<WaitingListMetaDataResponse> GetWaitingList()
    {
        var result = new WaitingListMetaDataResponse();
        var response = _waitingListService.GetWaitingList();
        result.Messages = response.Messages;
        if (response.Records.Count == 1)
        {
            result.WaitingList = response.Records.First();
        }
        return Ok(result);   
    }

    /// <summary>
    /// Retrieves the metadata associated with the waiting list.
    ///
    /// This method interacts with the service layer to obtain and return metadata
    /// </summary>
    /// <returns>An action result containing a response object with the metadata of the waiting list.</returns>
    [HttpGet]
    [Route("default-waiting-list")]
    public ActionResult<WaitingListMetaDataResponse> GetDefaultWaitingList()
    {
        var result = new WaitingListMetaDataResponse();
        var response = _waitingListService.GetWaitingList();
        result.Messages = response.Messages;
        if (response.Records.Count == 1)
        {
            result.WaitingList = response.Records.First();
        }
        return Ok(result);
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
        
        return Ok(_waitingListService.CheckIn(request.Party));
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

        if (request.Party == null)
        {   
            return BadRequest("No party provided"); 
        }

        return Ok(_waitingListService.AddPartyToWaitingList(request.Party));
    }
}