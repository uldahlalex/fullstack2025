using System.Collections.Concurrent;
using Fleck;
using StackExchange.Redis;

namespace Infrastructure.Websocket;



public class RedisConnectionRepository(IConnectionMultiplexer redis) : IRedisConnectionRepository
{
    public ConcurrentDictionary<string, object> LocalConnections { get; set; } = new();
    private readonly string _instanceId = Guid.NewGuid().ToString();
    

    public async Task<bool> RegisterConnection<T>(string connectionId, T connection)
    {
        var localAdd = LocalConnections.TryAdd(connectionId, connection);
        var db = redis.GetDatabase();
        var redisAdd = await db.HashSetAsync("connections", connectionId, _instanceId);
        return localAdd && redisAdd;
    }

    public async Task UnregisterConnection(string connectionId)
    {
        LocalConnections.TryRemove(connectionId, out _);
        var db = redis.GetDatabase();
        
        await db.HashDeleteAsync("connections", connectionId);
        
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



    public async Task BroadcastToTopic<T>(string topic, Action<List<T>> broadcastAction)
    {
        var db = redis.GetDatabase();
        
        // Get all connection IDs subscribed to this topic
        var subscriberIds = await db.SetMembersAsync($"topic:{topic}");
        
        // Filter for connections on this instance
        var localConnections = subscriberIds
            .Select(id => id.ToString())
            .Where(id => this.LocalConnections.ContainsKey(id))
            .Select(id => this.LocalConnections[id])
            .ToList();

        if (localConnections.Any())
        {
            if(localConnections is List<T> connections)
            {
                broadcastAction(connections);
            }
        }
        
    }
}