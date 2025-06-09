using System.Collections.Concurrent;
using System.Threading.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WaitingList.Contracts.DTOs;

namespace WaitingList.SseManager.Managers;

/// <summary>
/// Manages Server-Sent Events (SSE) communication channels for different client sessions.
/// This class facilitates the creation, retrieval, and management of unbounded communication channels
/// to send SSE messages to clients.
/// </summary>
public class SseChannelManager
{
    /// <summary>
    /// A thread-safe collection that manages communication channels for Server-Sent Events (SSE).
    /// Each channel is uniquely associated with a session ID and is used to send string messages to connected clients.
    /// </summary>
    private readonly ConcurrentDictionary<string, Channel<string>> _channels = new();

    /// <summary>
    /// Retrieves an existing communication channel associated with the specified session ID, or creates a new one if it does not already exist.
    /// The channel is used to facilitate sending Server-Sent Events (SSE) messages to a client.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the channel is being retrieved or created.</param>
    /// <returns>A channel instance that allows sending of string messages associated with the specified session ID.</returns>
    public Channel<string> GetOrCreateChannel(string sessionId)
    {
        return _channels.GetOrAdd(sessionId, _ => Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }));
    }

    /// <summary>
    /// Sends the specified Server-Sent Events (SSE) data transfer object (DTO) to the channel associated with the given session ID.
    /// Formats the DTO as an SSE-compliant string and writes it to the appropriate channel's writer.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session associated with the target communication channel.</param>
    /// <param name="dto">The Server-Sent Events DTO containing data and metadata to be sent to the target session's channel.</param>
    /// <typeparam name="T">The type of the data contained within the DTO.</typeparam>
    public void SendDto<T>(string sessionId, SseDto<T> dto)
    {
        var json = CreateJson(dto);
        var sseFormatted = $"data: {json}\n\n";

        if (_channels.TryGetValue(sessionId, out var channel))
        {
            channel.Writer.TryWrite(sseFormatted);
        }
    }

    /// <summary>
    /// Broadcasts the specified data transfer object (DTO) to all active communication channels.
    /// Converts the DTO into a server-sent events (SSE) formatted string and writes it to each channel's writer.
    /// </summary>
    /// <param name="dto">The server-sent events DTO containing data and its associated metadata to be broadcasted.</param>
    /// <typeparam name="T">The type of the data contained within the DTO.</typeparam>
    public void BroadcastDto<T>(SseDto<T> dto)
    {
        var json = CreateJson(dto);
        var sseFormatted = $"data: {json}\n\n";

        foreach (var channel in _channels.Values)
        {
            channel.Writer.TryWrite(sseFormatted);
        }
    }

    /// <summary>
    /// Removes the communication channel associated with the specified session ID and marks the channel's writer as complete.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session whose channel should be removed.</param>
    public void RemoveChannel(string sessionId)
    {
        if (_channels.TryRemove(sessionId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }

    /// <summary>
    /// Serializes an instance of <see cref="SseDto{T}"/> into a JSON string using specific serializer settings.
    /// </summary>
    /// <typeparam name="T">The type of the data contained within the <see cref="SseDto{T}"/>.</typeparam>
    /// <param name="dto">The instance of <see cref="SseDto{T}"/> to be serialized.</param>
    /// <returns>A JSON string representation of the <paramref name="dto"/> object. Returns an empty string if serialization fails.</returns>
    private string CreateJson<T>(SseDto<T> dto)
    {
        var result = string.Empty;
        try
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };

            result = JsonConvert.SerializeObject(dto, settings);
            Console.WriteLine("Sse manager: Json created");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SSE JSON Error: {ex.Message}");
        }

        return result;
    }
}

