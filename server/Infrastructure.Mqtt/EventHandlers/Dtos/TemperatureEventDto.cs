using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public record TemperatureEventDto : IMqttEventDto
{
    public double Temperature { get; init; }
    public string SensorId { get; init; } = null!;
    public string Topic { get; init; } = null!;
    public DateTime Timestamp { get; init; }
}