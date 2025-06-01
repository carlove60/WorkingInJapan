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
[Route("api/waiting-list")]
public class WaitingListController : ControllerBase
{
    private readonly IWaitingListService _waitingListService;
    private readonly IPartyService _partyService;

    /// <summary>
    /// Controller responsible for handling operations on a waiting list.
    /// Provides endpoints for retrieving the current waiting list, managing metadata,
    /// and adding parties to the waiting list.
    /// </summary>
    public WaitingListController(IWaitingListService waitingListService, IPartyService partyService)
    {
        this._waitingListService = waitingListService;
        this._partyService = partyService;   
    }
    
    /// <summary>
    /// Retrieves the metadata associated with the waiting list.
    ///
    /// This method interacts with the service layer to obtain and return metadata
    /// </summary>
    /// <returns>An action result containing a response object with the metadata of the waiting list.</returns>
    [HttpGet]
    [Route("waiting-list")]
    public ActionResult<WaitingListResponse> GetWaitingList()
    {
        var result = new WaitingListResponse();
        var response = _waitingListService.GetWaitingList(WaitingListBackend.Constants.DefaultWaitingListName);
        result.Messages = response.Messages;
        if (response.Records.Count == 1)
        {
            result.WaitingList = response.Records.First();
        }
        return Ok(result);
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

        request.Party.SessionId = HttpContext.Session.GetSessionId();
        var party = _partyService.GetParty(request.Party.SessionId);
        if (party.Records.Count > 0)
        {
            return Ok(party);
        }

        return Ok(_waitingListService.AddPartyToWaitingList(request.Party));
    }
}