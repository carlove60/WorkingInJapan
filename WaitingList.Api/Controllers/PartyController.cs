using Microsoft.AspNetCore.Mvc;
using WaitingList.Extensions;
using WaitingList.Responses;
using WaitingListBackend.Interfaces;

namespace WaitingList.Controllers;


/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/party")]
public class PartyController : ControllerBase
{
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

        return Ok(_partyService.CheckIn(sessionId));
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
