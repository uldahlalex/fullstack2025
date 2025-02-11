using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt;

public record HumidityEvent : IMqttEvent
{
    public double Humidity { get; init; }
    public string SensorId { get; init; }
    public string Topic { get; init; }
    public DateTime Timestamp { get; init; }
}