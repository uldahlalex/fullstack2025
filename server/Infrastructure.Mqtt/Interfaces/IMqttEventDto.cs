namespace Infrastructure.Mqtt.Interfaces;

public abstract record IMqttEventDto
{
    private string Topic { get; }
    private DateTime Timestamp { get; }
}