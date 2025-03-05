using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public class MetricEventDto : IMqttEventDto
{
    public double Value { get; set; }
    public string Unit { get; set; }
}