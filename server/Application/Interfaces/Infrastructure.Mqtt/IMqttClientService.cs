public interface IMqttClientService
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task SubscribeAsync(string topic);
    Task UnsubscribeAsync(string topic);
    IReadOnlyCollection<string> GetSubscribedTopics();
}