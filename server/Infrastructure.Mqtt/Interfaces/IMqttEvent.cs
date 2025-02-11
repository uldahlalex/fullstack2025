namespace Infrastructure.Mqtt.Interfaces;

public interface IMqttEvent
{
    string Topic { get; }
    DateTime Timestamp { get; }
}