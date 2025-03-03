using System.Collections.Concurrent;
using System.Text.Json;
using Fleck;
using Microsoft.Extensions.Logging;
using WebSocketBoilerplate;

namespace Api.WebSockets
{
    public class WebSocketConnectionManager<TConnection, TMessageBase> : IConnectionManager
        where TConnection : class
        where TMessageBase : class
    {
        private readonly ILogger<WebSocketConnectionManager<TConnection, TMessageBase>> _logger;

        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager<TConnection, TMessageBase>> logger)
        {
            _logger = logger;
        }

        private readonly ConcurrentDictionary<string /* Client ID */, TConnection> _connectionIdToSocket = new();
        private readonly ConcurrentDictionary<string /* Socket ID */, string /* Client ID */> _socketToConnectionId = new();
        
        public ConcurrentDictionary<string, object> ConnectionIdToSocket => 
            new(_connectionIdToSocket.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value));
        
        public ConcurrentDictionary<string, string> SocketToConnectionId => _socketToConnectionId;
        public ConcurrentDictionary<string, HashSet<string>> TopicMembers { get; set; } = new();
        public ConcurrentDictionary<string, HashSet<string>> MemberTopics { get; set; } = new();

        public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllTopicsWithMembers()
        {
            return Task.FromResult(TopicMembers);
        }

        public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllMembersWithTopics()
        {
            return Task.FromResult(MemberTopics);
        }

        public Task<Dictionary<string, string>> GetAllConnectionIdsWithSocketId()
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in _connectionIdToSocket)
            {
                result[kvp.Key] = GetSocketId(kvp.Value);
            }
            return Task.FromResult(result);
        }

        public Task<Dictionary<string, string>> GetAllSocketIdsWithConnectionId()
        {
            return Task.FromResult(_socketToConnectionId.ToDictionary(k => k.Key, v => v.Value));
        }

        public async Task AddToTopic(string topic, string memberId, TimeSpan? expiry = null)
        {
            TopicMembers.AddOrUpdate(
                topic,
                _ => new HashSet<string> { memberId },
                (_, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(memberId);
                        return existing;
                    }
                });

            MemberTopics.AddOrUpdate(
                memberId,
                _ => new HashSet<string> { topic },
                (_, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(topic);
                        return existing;
                    }
                });

            await LogCurrentState();

            if (expiry.HasValue)
            {
                _ = Task.Delay(expiry.Value).ContinueWith(async _ =>
                {
                    await RemoveFromTopic(topic, memberId);
                    _logger.LogInformation($"Removed member {memberId} from topic {topic} due to expiry");
                });
            }
        }

        public async Task RemoveFromTopic(string topic, string memberId)
        {
            if (TopicMembers.TryGetValue(topic, out var members))
            {
                lock (members)
                {
                    members.Remove(memberId);

                    if (members.Count == 0)
                    {
                        TopicMembers.TryRemove(topic, out _);
                    }
                }
            }

            if (MemberTopics.TryGetValue(memberId, out var topics))
            {
                lock (topics)
                {
                    topics.Remove(topic);

                    if (topics.Count == 0)
                    {
                        MemberTopics.TryRemove(memberId, out _);
                    }
                }
            }

            await LogCurrentState();
        }

        public Task<List<string>> GetMembersFromTopicId(string topic)
        {
            return Task.FromResult(
                TopicMembers.TryGetValue(topic, out var members)
                    ? new List<string>(members)
                    : new List<string>());
        }

        public Task<List<string>> GetTopicsFromMemberId(string memberId)
        {
            return Task.FromResult(
                MemberTopics.TryGetValue(memberId, out var topics)
                    ? new List<string>(topics)
                    : new List<string>());
        }

        public Task<string> GetClientIdFromSocketId(string socketId)
        {
            if (_socketToConnectionId.TryGetValue(socketId, out var connectionId))
            {
                return Task.FromResult(connectionId);
            }
            
            throw new KeyNotFoundException($"Could not find client ID for socket ID {socketId}");
        }

        public async Task OnOpen<T>(T socket, string clientId)
        {
            if (socket is not TConnection typedSocket)
            {
                throw new ArgumentException($"Expected socket of type {typeof(TConnection).Name} but got {typeof(T).Name}");
            }
            
            _logger.LogDebug($"OnOpen called with clientId: {clientId}");

            if (_connectionIdToSocket.TryRemove(clientId, out var oldSocket))
            {
                var oldSocketId = GetSocketId(oldSocket);
                _socketToConnectionId.TryRemove(oldSocketId, out _);
                _logger.LogInformation($"Removed old connection {oldSocketId} for client {clientId}");
            }

            _connectionIdToSocket[clientId] = typedSocket;
            _socketToConnectionId[GetSocketId(typedSocket)] = clientId;

            _logger.LogInformation($"Added new connection {GetSocketId(typedSocket)} for client {clientId}");
            await LogCurrentState();
        }

        public async Task OnClose<T>(T socket, string clientId)
        {
            if (socket is not TConnection typedSocket)
            {
                throw new ArgumentException($"Expected socket of type {typeof(TConnection).Name} but got {typeof(T).Name}");
            }
            
            var socketId = GetSocketId(typedSocket);
            _logger.LogDebug($"OnClose called with clientId: {clientId} and socketId: {socketId}");

            if (_connectionIdToSocket.TryGetValue(clientId, out var currentSocket) &&
                GetSocketId(currentSocket) == socketId)
            {
                _connectionIdToSocket.TryRemove(clientId, out _);
                _logger.LogInformation($"Removed connection for client {clientId}");
            }

            _socketToConnectionId.TryRemove(socketId, out _);

            if (MemberTopics.TryGetValue(clientId, out var topics))
            {
                foreach (var topic in new List<string>(topics))
                {
                    await RemoveFromTopic(topic, clientId);

                    if (TopicMembers.TryGetValue(topic, out var members) && members.Count > 0)
                    {
                        await NotifyMemberLeft(topic, clientId);
                    }
                }
            }

            MemberTopics.TryRemove(clientId, out _);
            await LogCurrentState();
        }

        public async Task BroadcastToTopic<TMessage>(string topic, TMessage message) where TMessage : class
        {
            await LogCurrentState();
            
            if (!TopicMembers.TryGetValue(topic, out var members))
            {
                _logger.LogWarning($"No topic found: {topic}");
                return;
            }

            var membersList = new List<string>(members);
            foreach (var memberId in membersList)
            {
                await BroadcastToMember(topic, memberId, message);
            }
        }

        private async Task BroadcastToMember<TMessage>(string topic, string memberId, TMessage message) where TMessage : class
        {
            if (!_connectionIdToSocket.TryGetValue(memberId, out var socket))
            {
                _logger.LogWarning($"No socket found for member: {memberId}");
                await RemoveFromTopic(topic, memberId);
                return;
            }

            if (!IsSocketAvailable(socket))
            {
                _logger.LogWarning($"Socket not available for {memberId}");
                await RemoveFromTopic(topic, memberId);
                return;
            }

            try
            {
                SendToSocket(socket, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to {memberId}");
                await RemoveFromTopic(topic, memberId);
            }
        }

        protected virtual string GetSocketId(TConnection socket)
        {

            if (socket is IWebSocketConnection webSocket)
            {
                return webSocket.ConnectionInfo.Id.ToString();
            }

            return socket.GetHashCode().ToString();
        }

        protected virtual bool IsSocketAvailable(TConnection socket)
        {

            if (socket is IWebSocketConnection webSocket)
            {
                return webSocket.IsAvailable;
            }

            return true;
        }

        protected virtual void SendToSocket<TMessage>(TConnection socket, TMessage message) 
            where TMessage : class
        {
            try
            {

                if (socket is IWebSocketConnection webSocket)
                {
                    if (message is TMessageBase baseMessage && typeof(TMessageBase) == typeof(BaseDto))
                    {


                        dynamic dynamicMessage = baseMessage;
                        webSocket.SendDto((BaseDto)dynamicMessage);
                        _logger.LogDebug($"Sent BaseDto message to socket {GetSocketId(socket)}");
                        return;
                    }

                    var json = JsonSerializer.Serialize(message);
                    webSocket.Send(json);
                    _logger.LogDebug($"Sent JSON message to socket {GetSocketId(socket)}");
                    return;
                }

                throw new NotImplementedException(
                    $"Sending messages to {typeof(TConnection).Name} with message type {typeof(TMessage).Name} " +
                    $"is not implemented. Override SendToSocket in a derived class.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to socket {GetSocketId(socket)}");
                throw;
            }
        }

        protected virtual async Task NotifyMemberLeft(string topic, string memberId)
        {
         
                var dto = new MemberLeftNotification
                {
                    ClientId = memberId, 
                    Topic = topic
                };

                await BroadcastToTopic(topic, dto);
        }

    

        public async Task LogCurrentState()
        {
            try
            {
                _logger.LogDebug(JsonSerializer.Serialize(new
                {
                    ConnectionIdToSocket = await GetAllConnectionIdsWithSocketId(),
                    SocketToConnectionId = await GetAllSocketIdsWithConnectionId(),
                    TopicsWithMembers = TopicMembers,
                    MembersWithTopics = MemberTopics
                }, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging current state");
            }
        }
    }

    public class MemberLeftNotification : BaseDto
    {
        public string ClientId { get; set; }
        public string Topic { get; set; }
    }
}