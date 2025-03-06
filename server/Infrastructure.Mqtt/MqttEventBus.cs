using MQTTnet;
using System.Text;
using System.Text.Json;

public class TopicPattern
{
    private readonly string _pattern;
    private readonly string[] _segments;

    public TopicPattern(string pattern)
    {
        _pattern = pattern;
        _segments = pattern.Split('/');
    }

    public bool Matches(string topic, out Dictionary<string, string> parameters)
    {
        parameters = new Dictionary<string, string>();
        var topicSegments = topic.Split('/');

        if (topicSegments.Length != _segments.Length)
            return false;

        for (int i = 0; i < _segments.Length; i++)
        {
            if (_segments[i] == "+")
            {
                parameters[i.ToString()] = topicSegments[i];
                continue;
            }

            if (_segments[i] != topicSegments[i])
                return false;
        }

        return true;
    }
}

public class MqttEvent
{
    public string Topic { get; set; }
    public string Payload { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}

public class MqttEventBus
{
    private readonly List<(TopicPattern Pattern, Action<MqttEvent> Handler)> _subscribers 
        = new List<(TopicPattern, Action<MqttEvent>)>();
    private readonly IMqttClient _client;

    public MqttEventBus(IMqttClient client)
    {
        _client = client;
        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }

    public async Task SubscribeAsync(string topicPattern, Action<MqttEvent> handler)
    {
        await _client.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topicPattern)
            .Build());
        _subscribers.Add((new TopicPattern(topicPattern), handler));
    }

    private Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        
        foreach (var (pattern, handler) in _subscribers)
        {
            if (pattern.Matches(topic, out var parameters))
            {
                var mqttEvent = new MqttEvent
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    Parameters = parameters
                };

                handler(mqttEvent);
            }
        }

        return Task.CompletedTask;
    }
}

