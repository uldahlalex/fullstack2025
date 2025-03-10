using System.Text;
using MQTTnet;

namespace Infrastructure.Mqtt;

public class MqttEventBus
{
    private readonly IMqttClient _mqttClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttEventBus> _logger;

    public MqttEventBus(IMqttClient mqttClient, IServiceProvider serviceProvider, ILogger<MqttEventBus> logger)
    {
        _mqttClient = mqttClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }



    public async Task SubscribeWithHandlerAsync( IEnumerable<IMqttEventHandler> handlers)
    {
        foreach(var handler in handlers)
        {
            await _mqttClient.SubscribeAsync(handler.TopicPattern);

        _mqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            if (TopicMatchesPattern(e.ApplicationMessage.Topic, handler.TopicPattern))
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var parameters = ExtractTopicParameters(e.ApplicationMessage.Topic, handler.TopicPattern);
                
                var mqttEvent = new MqttEvent
                {
                    Topic = e.ApplicationMessage.Topic,
                    Payload = payload,
                    Parameters = parameters
                };

                using var scope = _serviceProvider.CreateScope();
                var scopedHandler = scope.ServiceProvider.GetRequiredService(handler.GetType()) as IMqttEventHandler;
                await scopedHandler.HandleAsync(mqttEvent);
            }
        };
        }
        
    }

    // Utility methods for topic matching
    private bool TopicMatchesPattern(string topic, string pattern) 
    {
        // Implementation for MQTT topic pattern matching
        // Simple implementation - can be enhanced for more complex patterns
        var patternParts = pattern.Split('/');
        var topicParts = topic.Split('/');
        
        if (patternParts.Length != topicParts.Length) return false;
        
        for (int i = 0; i < patternParts.Length; i++)
        {
            if (patternParts[i] == "+" || patternParts[i] == "#") continue;
            if (patternParts[i] != topicParts[i]) return false;
        }
        
        return true;
    }

    private Dictionary<string, string> ExtractTopicParameters(string topic, string pattern)
    {
        var parameters = new Dictionary<string, string>();
        var patternParts = pattern.Split('/');
        var topicParts = topic.Split('/');
        
        for (int i = 0; i < patternParts.Length; i++)
        {
            if (patternParts[i] == "+")
            {
                parameters["param" + i] = topicParts[i];
            }
        }
        
        return parameters;
    }
}