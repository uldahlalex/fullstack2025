namespace Infrastructure.Mqtt.Interfaces;

/// <summary>
/// This is very much like the "BaseDto" for websockets (used for incoming traffic from the broker)
/// </summary>
public abstract class IMqttEventDto
{
    private string Topic { get; set; }
    private DateTime Timestamp { get; set; }
    public string DeviceId { get; set; }
}