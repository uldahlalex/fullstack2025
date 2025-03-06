using System.ComponentModel.DataAnnotations;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers.Dtos;

public class IoTDeviceSendsMetricEventDto : IMqttEventDto
{
    [Required]
    public double Value { get; set; }
    [Required]
    public string Unit { get; set; }
}