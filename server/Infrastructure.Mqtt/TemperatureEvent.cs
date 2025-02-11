using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt;

public record TemperatureEvent : IMqttEvent
{
    public double Temperature { get; init; }
    public string SensorId { get; init; }
    public string Topic { get; init; }
    public DateTime Timestamp { get; init; }
}