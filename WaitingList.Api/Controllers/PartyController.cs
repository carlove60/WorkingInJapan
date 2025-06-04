using Microsoft.AspNetCore.Mvc;
using WaitingList.Extensions;
using WaitingList.Responses;
using WaitingListBackend.Interfaces;

namespace WaitingList.Controllers;

/// <summary>
/// Controller responsible for managing operations related to the waiting list system.
/// Provides endpoints for operations such as checking in and retrieving party information.
/// </summary>
[ApiController]
[Route("api/party")]
public class PartyController : ControllerBase
{
    /// <summary>
    /// Service responsible for handling operations related to party data in the waiting list system.
    /// Provides methods for managing party information such as checking in and retrieving party details.
    /// </summary>
    private readonly IPartyService _partyService;

    /// <summary>
    /// Controller responsible for handling operations on a waiting list.
    /// Provides endpoints for retrieving the current waiting list, managing metadata,
    /// and adding parties to the waiting list.
    /// </summary>
    public PartyController(IPartyService partyService)
    {
        _partyService = partyService;
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
    public ActionResult<CheckInResponse> CheckIn()
    {
        var sessionId = HttpContext.Session.GetSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("No session found, please fill in your name on the waiting list first");
        }

        var result = _partyService.CheckIn(sessionId);
        var response = new CheckInResponse();
        response.Messages = result.Messages;
        if (result.Records.Count == 1)
        {
            response.Party = result.Records.First();
        }

        return Ok(response);
    }

    /// <summary>
    /// Cancels an existing check-in for a session based on the session ID.
    /// Validates the session, retrieves the relevant party data, and removes the check-in if applicable.
    /// </summary>
    /// <returns>A response containing the updated party information and any validation messages.</returns>
    [Route("cancel-check-in")]
    [HttpPost]
    public ActionResult<CancelCheckInResponse> CancelCheckIn()
    {
        var sessionId = HttpContext.Session.GetSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("No session found, please fill in your name on the waiting list first");
        }

        var result = _partyService.CancelCheckIn(sessionId);
        var response = new CancelCheckInResponse();
        response.Messages = result.Messages;
        return Ok(response);
    }


    /// <summary>
    /// Retrieves information about the party associated with the current session.
    /// If no session is found, returns a bad request response with an appropriate error message.
    /// </summary>
    /// <returns>A response containing the party details and any related messages.</returns>
    [HttpGet]
    [Route("/get-party")]
    public ActionResult<GetPartyResponse> GetParty()
    {
        var sessionId = HttpContext.Session.GetSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("No session found, please fill in your name on the waiting list first");
        }
        
        var result = new GetPartyResponse();
        var response = _partyService.GetParty(sessionId);
        result.Messages = response.Messages;
        if (response.Records.Count == 1)
        {
            result.Party = response.Records.First();
        }
        return Ok(result);   
    }
}
