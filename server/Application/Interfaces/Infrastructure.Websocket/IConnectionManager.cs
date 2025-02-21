using System.Collections.Concurrent;


namespace Api;

public interface IConnectionManager<T, TBaseDto>
{
    ConcurrentDictionary<string, T> ConnectionIdToSocket { get; }

    Task OnOpen(T socket, string clientId);
    Task OnClose(T socket, string clientId);
    Task BroadcastToTopic<TMessage>(string topic, TMessage message) where TMessage : TBaseDto;
    
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