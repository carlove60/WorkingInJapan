using Microsoft.AspNetCore.Mvc;
using WaitingList.Extensions;
using WaitingList.Responses;
using WaitingListBackend.Interfaces;

namespace WaitingList.Controllers;


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
    
    
    [HttpGet]
    [Route("/get-party")]
    public ActionResult<GetPartyResponse> GetParty()
    {
        var sessionId = HttpContext.Session.GetOrCreateSessionId();
        var result = new GetPartyResponse();
        var response = _partyService.GetParty(Guid.Parse(sessionId));;
        result.Messages = response.Messages;
        if (response.Records.Count == 1)
        {
            result.Party = response.Records.First();
        }
        return Ok(result);   
    }
}
