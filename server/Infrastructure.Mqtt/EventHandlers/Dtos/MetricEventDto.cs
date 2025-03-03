using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public record MetricEventDto : IMqttEventDto
{
    public double Value { get; set; }
    public string Unit { get; set; }
    public string SensorId { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}