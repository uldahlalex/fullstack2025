namespace Application.Interfaces.Infrastructure.Mqtt;

public interface IMqttClientService
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task SubscribeAsync(string topic);

    Task UnsubscribeAsync(string topic);
    //event Func<MqttMessage, Task> OnMessageReceived;
}