using System.Text.Json;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt;

public class EventDispatcher(
    ILogger<EventDispatcher> logger,
    IServiceProvider serviceProvider)
    : IEventDispatcher
{
    private readonly Dictionary<string, Type> _topicMappings = new()
    {
        { "sensors/+/temperature", typeof(TemperatureEvent) },
        { "sensors/+/humidity", typeof(HumidityEvent) }
    };


    public async Task DispatchAsync(string topic, string payload)
    {
        var eventType = GetEventTypeForTopic(topic);

        try
        {
            IMqttEvent mqttEvent = JsonSerializer.Deserialize(payload, eventType, new JsonSerializerOptions
                                   {
                                       PropertyNameCaseInsensitive = true
                                   }) as IMqttEvent ??
                                   throw new Exception("Could not pass object as IMqttEvent " + payload +
                                                       " with event type " + eventType);
            

            var handlerType = typeof(IMqttEventHandler<>).MakeGenericType(eventType);

            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService(handlerType) ??
                          throw new Exception("Could not find handler with name " + handlerType);

            var method = handler.GetType().GetMethod(nameof(IMqttEventHandler<MockMqttObject>.HandleAsync)) ??
                         throw new Exception("Could not find " + nameof(IMqttEventHandler<MockMqttObject>.HandleAsync));
            await (Task)method.Invoke(handler, [mqttEvent])!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message for topic {Topic}", topic);
        }
    }

    private Type GetEventTypeForTopic(string topic)
    {
        var matchingPattern = _topicMappings.Keys
                                  .FirstOrDefault(pattern => IsTopicMatch(topic, pattern)) ??
                              throw new Exception("Topic not found using: " + topic);
        return _topicMappings[matchingPattern];
    }

    private bool IsTopicMatch(string actualTopic, string pattern)
    {
        var actualParts = actualTopic.Split('/');
        var patternParts = pattern.Split('/');

        if (actualParts.Length != patternParts.Length) return false;

        for (var i = 0; i < actualParts.Length; i++)
            if (patternParts[i] != "+" && patternParts[i] != actualParts[i])
                return false;

        return true;
    }
}

public interface IEventDispatcher
{
    Task DispatchAsync(string topic, string payload);
}