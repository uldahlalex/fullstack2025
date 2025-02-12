
using System.Collections.Concurrent;
using StackExchange.Redis;


public class RedisConnectionRepository<T>(IConnectionMultiplexer redis) : IRedisConnectionRepository
{
    private readonly ConcurrentDictionary<string, T> _localConnections = new();
    private readonly string _instanceId = Guid.NewGuid().ToString(); // Unique ID for this server instance

    public async Task RegisterConnection(string connectionId, T connection)
    {
        _localConnections.TryAdd(connectionId, connection);
        var db = redis.GetDatabase();
        
        // Store connection-to-instance mapping
        await db.HashSetAsync("connections", connectionId, _instanceId);
    }

    public Task RegisterConnection(string connectionId, object connection)
    {
        throw new NotImplementedException();
    }

    public async Task UnregisterConnection(string connectionId)
    {
        _localConnections.TryRemove(connectionId, out _);
        var db = redis.GetDatabase();
        
        // Remove connection mapping
        await db.HashDeleteAsync("connections", connectionId);
        
        // Remove from all topics
        var topics = await db.SetMembersAsync($"connection:{connectionId}:topics");
        foreach (var topic in topics)
        {
            await db.SetRemoveAsync($"topic:{topic}", connectionId);
        }
        await db.KeyDeleteAsync($"connection:{connectionId}:topics");
    }

    public async Task SubscribeToTopic(string connectionId, string topic)
    {
        var db = redis.GetDatabase();
        
        // Add connection to topic
        await db.SetAddAsync($"topic:{topic}", connectionId);
        // Track topic subscription for connection
        await db.SetAddAsync($"connection:{connectionId}:topics", topic);
    }

    public async Task UnsubscribeFromTopic(string connectionId, string topic)
    {
        var db = redis.GetDatabase();
        
        await db.SetRemoveAsync($"topic:{topic}", connectionId);
        await db.SetRemoveAsync($"connection:{connectionId}:topics", topic);
    }

    public async Task BroadcastToTopic<T>(string topic, Action<IList<object>> broadcastAction)
    {
        var db = redis.GetDatabase();
        
        // Get all connection IDs subscribed to this topic
        var subscriberIds = await db.SetMembersAsync($"topic:{topic}");
        
        // Filter for connections on this instance
        var localConnections = subscriberIds
            .Select(id => id.ToString())
            .Where(id => _localConnections.ContainsKey(id))
            .Select(id => _localConnections[id])
            .ToList();

        if (localConnections.Any())
        {
            broadcastAction(localConnections);
        }
    }

    public async Task BroadcastToTopic(string topic, Action<IList<T>> broadcastAction)
    {
        var db = redis.GetDatabase();
        
        // Get all connection IDs subscribed to this topic
        var subscriberIds = await db.SetMembersAsync($"topic:{topic}");
        
        // Filter for connections on this instance
        var localConnections = subscriberIds
            .Select(id => id.ToString())
            .Where(id => _localConnections.ContainsKey(id))
            .Select(id => _localConnections[id])
            .ToList();

        if (localConnections.Any())
        {
            broadcastAction(localConnections);
        }
    }
}
