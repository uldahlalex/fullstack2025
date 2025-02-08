namespace Infrastructure.Mqtt.Interfaces;

public interface IMqttEvent
{
    string Topic { get; }
    DateTime Timestamp { get; }
}

public interface IMqttEventHandler<in T> where T : IMqttEvent
{
    Task HandleAsync(T eventData);
}

public class MockMqttObject : IMqttEvent
{
    public string Topic { get; }
    public DateTime Timestamp { get; }
}

public class MockMqttEvent : IMqttEventHandler<MockMqttObject>
{
    public Task HandleAsync(MockMqttObject eventData)
    {
        return Task.CompletedTask;
    }
}