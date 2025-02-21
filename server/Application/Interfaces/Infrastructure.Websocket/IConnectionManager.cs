using System.Collections.Concurrent;


namespace Api;

public interface IConnectionManager
{
    ConcurrentDictionary<string, string> SocketToConnectionId { get; }
    Task<Dictionary<string, string>> GetAllSocketIdsWithConnectionId();
    public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllTopicsWithMembers();
    public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllMembersWithTopics();
    public Task<Dictionary<string, string>> GetAllConnectionIdsWithSocketId();

    Task AddToTopic(string topic, string memberId, TimeSpan? expiry = null);
    Task RemoveFromTopic(string topic, string memberId);
    Task<List<string>> GetMembersFromTopicId(string topic);
    Task<List<string>> GetTopicsFromMemberId(string memberId);

}

public interface IConnectionManager<TConection> : IConnectionManager
{
    ConcurrentDictionary<string, TConection> ConnectionIdToSocket { get; }

    Task OnOpen(TConection socket, string clientId);
    Task OnClose(TConection socket, string clientId);
    Task BroadcastToTopic<T>(string topic, T message);
}