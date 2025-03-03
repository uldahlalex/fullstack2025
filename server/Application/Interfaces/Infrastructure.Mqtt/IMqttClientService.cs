namespace Application.Interfaces.Infrastructure.Mqtt;

public interface IMqttClientService
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task SubscribeAsync(string topic);
    Task UnsubscribeAsync(string topic);
    IReadOnlyCollection<string> GetSubscribedTopics();
    Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1);
    IEnumerable<string> GetSubscriptionTopics();
}