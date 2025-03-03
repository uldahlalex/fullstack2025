namespace Infrastructure.Mqtt.Interfaces;

/// <summary>
/// This is very much like the "BaseDto" for websockets (used for incoming traffic from the broker)
/// </summary>
public abstract record IMqttEventDto
{
    private string Topic { get; }
    private DateTime Timestamp { get; }
    public string DeviceId { get; }
}