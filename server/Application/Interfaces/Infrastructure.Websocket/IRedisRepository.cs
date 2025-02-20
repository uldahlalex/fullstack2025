using System.Collections.Concurrent;

namespace Application.Interfaces.Infrastructure.Websocket;

public interface IRedisConnectionRepository
{
    ConcurrentDictionary<string, object> LocalConnections { get; set; }
    Task<bool> RegisterConnection<T>(string connectionId, T connection);
    Task UnregisterConnection(string connectionId);
    Task SubscribeToTopic(string connectionId, string topic);
    Task UnsubscribeFromTopic(string connectionId, string topic);
    Task BroadcastToTopic<T>(string topic, Action<List<T>> broadcastAction);
}