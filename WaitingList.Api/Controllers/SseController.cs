using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WaitingList.Contracts.DTOs;
using WaitingList.Extensions;
using WaitingList.Responses;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Interfaces;

namespace WaitingList.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SseController : ControllerBase
{
    private readonly SseChannelManager _sseChannelManager;

    /// <summary>
    /// API controller for managing Server-Sent Events (SSE) endpoints.
    /// Handles streaming real-time updates to clients based on session contexts
    /// using the services and channel manager injected.
    /// </summary>
    public SseController(SseChannelManager sseChannelManager)
    {
        _sseChannelManager = sseChannelManager;   
    }

    /// <summary>
    /// Handles Server-Sent Events (SSE) for updating DTOs in real-time by streaming updates to the client.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation of sending streamed messages to the client over an HTTP response.</returns>
    [HttpGet("dto-update")]
    public async Task GetDtoUpdate()
    {
        AddHeaders(Response);
        var sessionId = HttpContext.Session.GetSessionId();
        var channel = _sseChannelManager.GetOrCreateChannel(sessionId);
        var cancellation = HttpContext.RequestAborted;

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellation))
            {
                await Response.WriteAsync(message, cancellation);
                await Response.Body.FlushAsync(cancellation);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected
        }
        finally
        {
            _sseChannelManager.RemoveChannel(sessionId);
        }
    }
    
    private async Task SendSseData<T>(HttpResponse response, T model, CancellationToken ct)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };

            var json = JsonConvert.SerializeObject(model, settings);
            await response.WriteAsync($"data: {json}\n\n", ct);
            await response.Body.FlushAsync(ct);
            Console.WriteLine("SSE: Flushed data to client.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SSE JSON Error: {ex.Message}");
        }
    }

    private void AddHeaders(HttpResponse response)
    {
        Console.WriteLine("SSE: Adding response headers...");
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");
        response.Headers.Add("X-Accel-Buffering", "no");
        response.Headers.Add("Access-Control-Allow-Credentials", "true");
        response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");

    }

    private bool IsAbleToCheckIn(PartyDto? partyDto)
    {
        return partyDto is { CanCheckIn: true, CheckedIn: false };
    }

}