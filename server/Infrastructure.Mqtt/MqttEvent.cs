namespace Infrastructure.Mqtt;

public class MqttEvent
{
    public string Topic { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public Dictionary<string, string> Parameters { get; set; } = new();
}