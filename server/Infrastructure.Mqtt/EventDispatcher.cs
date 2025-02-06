using System.Text.Json;

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
        if (eventType == null)
        {
            logger.LogWarning("No event type mapping found for topic: {Topic}", topic);
            return;
        }

        try
        {
            var mqttEvent = JsonSerializer.Deserialize(payload, eventType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) as IMqttEvent;

            if (mqttEvent == null)
            {
                logger.LogError("Failed to deserialize payload for topic: {Topic}", topic);
                return;
            }

            // Get the handler type for this event
            var handlerType = typeof(IMqttEventHandler<>).MakeGenericType(eventType);

            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService(handlerType);

            if (handler == null)
            {
                logger.LogWarning("No handler registered for event type: {EventType}", eventType.Name);
                return;
            }

            var method = handler.GetType().GetMethod("HandleAsync");
            await (Task)method.Invoke(handler, new[] { mqttEvent });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message for topic {Topic}", topic);
        }
    }

    private Type GetEventTypeForTopic(string topic)
    {
        var matchingPattern = _topicMappings.Keys
            .FirstOrDefault(pattern => IsTopicMatch(topic, pattern));
        return matchingPattern != null ? _topicMappings[matchingPattern] : null;
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