using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public record HumidityEventDto : IMqttEventDto
{
    public double Humidity { get; init; }
    public string SensorId { get; init; } = null!;
    public string Topic { get; init; } = null!;
    public DateTime Timestamp { get; init; }
}