using System.Text;
using MQTTnet;

namespace Infrastructure.Mqtt;

public class MqttEventBus
{
    private readonly ILogger<MqttEventBus> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly IServiceProvider _serviceProvider;

    public MqttEventBus(IMqttClient mqttClient, IServiceProvider serviceProvider, ILogger<MqttEventBus> logger)
    {
        _mqttClient = mqttClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RegisterHandlersAsync(IEnumerable<IMqttEventHandler> handlers)
    {
        var topicHandlerMap = new Dictionary<string, Type>();
        
        foreach (var handler in handlers)
        {
            var handlerType = handler.GetType();
            var topicPattern = handler.TopicPattern;
            
            topicHandlerMap[topicPattern] = handlerType;
            await _mqttClient.SubscribeAsync(topicPattern);
            _logger.LogInformation($"Subscribed to topic pattern: {topicPattern} with handler {handlerType.Name}");
        }
        
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            
            foreach (var (pattern, handlerType) in topicHandlerMap)
            {
                if (!TopicMatchesPattern(topic, pattern)) continue;
                
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var parameters = ExtractTopicParameters(topic, pattern);

                var mqttEvent = new MqttEvent
                {
                    Topic = topic,
                    Payload = payload,
                    Parameters = parameters
                };

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService(handlerType) as IMqttEventHandler;
                
                if (handler == null)
                {
                    _logger.LogError($"Failed to resolve handler of type {handlerType.Name}");
                    continue;
                }
                
                _logger.LogInformation($"Processing message on topic {topic} with handler {handlerType.Name}");
                await handler.HandleAsync(mqttEvent);
                
                break;
            }
        };
    }

    private bool TopicMatchesPattern(string topic, string pattern)
    {
        var patternParts = pattern.Split('/');
        var topicParts = topic.Split('/');

        if (patternParts.Length != topicParts.Length) return false;

        for (var i = 0; i < patternParts.Length; i++)
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

        for (var i = 0; i < patternParts.Length; i++)
            if (patternParts[i] == "+")
                parameters["param" + i] = topicParts[i];

        return parameters;
    }
}