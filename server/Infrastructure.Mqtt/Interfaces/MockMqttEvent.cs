namespace Infrastructure.Mqtt.Interfaces;

public class MockMqttEvent : IMqttEventHandler<MockMqttObject>
{
    public Task HandleAsync(MockMqttObject eventData)
    {
        return Task.CompletedTask;
    }
}