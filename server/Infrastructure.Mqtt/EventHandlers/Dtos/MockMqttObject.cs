using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public record MockMqttObject : IMqttEventDto
{
    public string Topic { get; }
    public DateTime Timestamp { get; }
}