using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Websocket.Client;
using WebSocketBoilerplate;
using Xunit.Abstractions;

namespace Fix;

public class WsRequestClient
{
    public readonly WebsocketClient Client;
    public readonly List<BaseDto> ReceivedMessages = new();
   private readonly Assembly[] _assemblies;

   private readonly ITestOutputHelper? _outputHelper;
   //initialize a logger
   // private readonly ILogger _logger = new LoggerFactory().CreateLogger("WsRequestClient");

    /// <summary>
    /// Defaults to ws://localhost:8181 if no other url string is specified
    /// </summary>
    /// <param name="url"></param>
    /// <param name="assemblies">Assemblies containing the DTO types</param>
    public WsRequestClient(Assembly[] assemblies,string? url = "ws://localhost:8181",  ITestOutputHelper? outputHelper = null)
    {
        _outputHelper = outputHelper;
        _assemblies = assemblies.Length > 0 
            ? assemblies 
            : new[] { Assembly.GetExecutingAssembly() };

        Client = new WebsocketClient(new Uri(url ?? "ws://localhost:8181"));

          Client.MessageReceived.Subscribe(msg =>
    {
        try 
        {
            _outputHelper?.WriteLine($"Raw message received: {msg.Text}");

            var baseDto = JsonSerializer.Deserialize<BaseDto>(msg.Text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (baseDto == null)
            {
                _outputHelper?.WriteLine("Failed to deserialize as BaseDto");
                return;
            }

            _outputHelper?.WriteLine($"Deserialized base DTO - EventType: {baseDto.eventType}, RequestId: {baseDto.requestId}");

            var eventType = baseDto.eventType.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)
                ? baseDto.eventType
                : baseDto.eventType + "Dto";

            _outputHelper?.WriteLine($"Looking for type: {eventType}");

            // Log available types in assemblies
            foreach (var assembly in _assemblies)
            {
                _outputHelper?.WriteLine($"Searching in assembly: {assembly.FullName}");
                var types = assembly.GetTypes()
                    .Where(t => typeof(BaseDto).IsAssignableFrom(t))
                    .Select(t => t.Name);
                _outputHelper?.WriteLine($"Available DTO types: {string.Join(", ", types)}");
            }

            var dtoType = _assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(eventType, StringComparison.OrdinalIgnoreCase));

            if (dtoType == null)
            {
                _outputHelper?.WriteLine($"Could not find type for event: {eventType}");
                return;
            }

            _outputHelper?.WriteLine($"Found type: {dtoType.FullName}");

            var fullDto = JsonSerializer.Deserialize(msg.Text, dtoType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) as BaseDto;

            if (fullDto != null)
            {
                _outputHelper?.WriteLine($"Successfully deserialized to {dtoType.Name}");
                lock (ReceivedMessages)
                {
                    ReceivedMessages.Add(fullDto);
                    _outputHelper?.WriteLine($"Added message to ReceivedMessages. Total count: {ReceivedMessages.Count}");
                }
            }
            else
            {
                _outputHelper?.WriteLine("Failed to deserialize as specific DTO type");
            }
        }
        catch (Exception ex)
        {
            _outputHelper?.WriteLine($"Error processing message: {ex}");
            _outputHelper?.WriteLine($"Message was: {msg.Text}");
        }
    });
    }


    public async Task<WsRequestClient> ConnectAsync()
    {
        try
        {
            Client.ReconnectTimeout = null; // Disable auto reconnection
            await Client.Start();
            if (!Client.IsRunning)
            {
                throw new Exception("WebSocket client failed to start");
            }

            return this;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to connect: {ex.Message}", ex);
        }
    }

   
    private void Send<T>(T dto) where T : BaseDto
    {
        var serialized = JsonSerializer.Serialize(dto, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        Client.Send(serialized);
    }

    /// <summary>
    /// For sending a message without expecing a response
    /// </summary>
    /// <param name="sendDto"></param>
    /// <typeparam name="T"></typeparam>
    public async Task SendMessage<T>(T sendDto) where T : BaseDto
    {
        Send(sendDto);
    }

    /// <summary>
    /// When a response is expected
    /// </summary>
    /// <param name="sendDto"></param>
    /// <param name="timeoutSeconds">Defaults to 7 seconds. Supply int param to change</param>
    /// <typeparam name="T">The sending DTO type</typeparam>
    /// <typeparam name="TR">The responding DTO type</typeparam>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
 public async Task<TR> SendMessage<T, TR>(T sendDto, int timeoutSeconds = 7) 
        where T : BaseDto 
        where TR : BaseDto
    {
        if (string.IsNullOrEmpty(sendDto.requestId))
        {
            sendDto.requestId = Guid.NewGuid().ToString();
        }
        
        _outputHelper?.WriteLine($"Sending message with requestId: {sendDto.requestId}");
        Send(sendDto);

        var startTime = DateTime.UtcNow;
        while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(timeoutSeconds))
        {
            lock (ReceivedMessages)
            {
                _outputHelper?.WriteLine($"Checking messages. Total messages: {ReceivedMessages.Count}");
                var responses = GetMessagesOfType<TR>().ToList();
                _outputHelper?.WriteLine($"Found {responses.Count} messages of type {typeof(TR).Name}");
                
                foreach (var response in responses)
                {
                    _outputHelper?.WriteLine($"Response RequestId: {response.requestId}, EventType: {response.eventType}");
                }
                
                var filteredByRequestId = responses.FirstOrDefault(msg => msg.requestId == sendDto.requestId);
                if (filteredByRequestId != null)
                {
                    _outputHelper?.WriteLine($"Found matching response!");
                    return filteredByRequestId;
                }
            }
            await Task.Delay(100);
        }

        _outputHelper?.WriteLine($"Timeout reached. Dumping all received messages:");
        lock (ReceivedMessages)
        {
            foreach (var msg in ReceivedMessages)
            {
                _outputHelper?.WriteLine($"Message Type: {msg.GetType().Name}, RequestId: {msg.requestId}, EventType: {msg.eventType}");
            }
        }

        throw new TimeoutException($"Did not receive expected response of type {typeof(TR).Name} with requestId {sendDto.requestId} within {timeoutSeconds} seconds");
    }
    public IEnumerable<T> GetMessagesOfType<T>() where T : BaseDto
    {
        lock (ReceivedMessages)
        {
            return ReceivedMessages
                .Where(msg => msg is T)
                .Cast<T>()
                .ToList();
        }
    }
}