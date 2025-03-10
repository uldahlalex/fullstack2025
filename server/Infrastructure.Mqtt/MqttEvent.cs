public class MqttEvent
{
    public string Topic { get; set; }
    public string Payload { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}