using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers;

public class MockMqttEventHandler : IMqttEventHandler<MockMqttObject>
{
    public Task HandleAsync(MockMqttObject eventData)
    {
        return Task.CompletedTask;
    }
}