using System.Collections.Concurrent;
public interface IRedisConnectionRepository
{
    Task RegisterConnection(string connectionId, object connection);
    Task UnregisterConnection(string connectionId);
    Task SubscribeToTopic(string connectionId, string topic);
    Task UnsubscribeFromTopic(string connectionId, string topic);
    Task BroadcastToTopic(string topic, Action<IList<object>> broadcastAction);
}
