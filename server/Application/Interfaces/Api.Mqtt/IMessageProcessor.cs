public interface IMqttClientConnection
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken);
    Task DisconnectAsync(CancellationToken cancellationToken);
    Task SubscribeAsync(string topic);
    Task UnsubscribeAsync(string topic);
    event Func<MqttMessage, Task> OnMessageReceived;
}

public class MqttMessage
{
    public string Topic { get; set; }
    public string Payload { get; set; }
    public DateTime Timestamp { get; set; }
}