namespace Infrastructure.Mqtt;

public interface IMqttEventHandler
{
    string TopicPattern { get; }
    Task HandleAsync(MqttEvent evt);
}