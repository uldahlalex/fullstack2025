using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Mqtt.Interfaces;

/// <summary>
///     This is very much like the "BaseDto" for websockets (used for incoming traffic from the broker)
/// </summary>
public abstract class IMqttEventDto
{
    [Required]
    private string Topic { get; set; }
    [Required]
    public string DeviceId { get; set; }
    [Required]
    public string eventType { get; set; }
}