namespace Infrastructure.Mqtt.Interfaces;

public class MockMqttObject : IMqttEvent
{
    public string Topic { get; }
    public DateTime Timestamp { get; }
}