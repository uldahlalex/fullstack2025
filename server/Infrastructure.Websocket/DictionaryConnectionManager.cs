using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Fleck;
using Microsoft.Extensions.Logging;
using WebSocketBoilerplate;

namespace Api.WebSockets
{
    public class WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger) : IConnectionManager
    {
        private readonly ConcurrentDictionary<string /* Client ID */, IWebSocketConnection> _connectionIdToSocket = new();
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
                    logger.LogInformation($"Removed member {memberId} from topic {topic} due to expiry");
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
            if (socket is not IWebSocketConnection webSocket)
            {
                throw new ArgumentException($"Expected socket of type IWebSocketConnection but got {typeof(T).Name}");
            }
            
            logger.LogDebug($"OnOpen called with clientId: {clientId}");

            if (_connectionIdToSocket.TryRemove(clientId, out var oldSocket))
            {
                var oldSocketId = GetSocketId(oldSocket);
                _socketToConnectionId.TryRemove(oldSocketId, out _);
                logger.LogInformation($"Removed old connection {oldSocketId} for client {clientId}");
            }

            _connectionIdToSocket[clientId] = webSocket;
            _socketToConnectionId[GetSocketId(webSocket)] = clientId;

            logger.LogInformation($"Added new connection {GetSocketId(webSocket)} for client {clientId}");
            await LogCurrentState();
        }

        public async Task OnClose<T>(T socket, string clientId)
        {
            if (socket is not IWebSocketConnection webSocket)
            {
                throw new ArgumentException($"Expected socket of type IWebSocketConnection but got {typeof(T).Name}");
            }
            
            var socketId = GetSocketId(webSocket);
            logger.LogDebug($"OnClose called with clientId: {clientId} and socketId: {socketId}");

            if (_connectionIdToSocket.TryGetValue(clientId, out var currentSocket) &&
                GetSocketId(currentSocket) == socketId)
            {
                _connectionIdToSocket.TryRemove(clientId, out _);
                logger.LogInformation($"Removed connection for client {clientId}");
            }

            _socketToConnectionId.TryRemove(socketId, out _);

            if (MemberTopics.TryGetValue(clientId, out var topics))
            {
                foreach (var topic in new List<string>(topics))
                {
                    await RemoveFromTopic(topic, clientId);

                    if (TopicMembers.TryGetValue(topic, out var members) && members.Count > 0)
                    {
                        var notification = new MemberLeftNotification 
                        { 
                            MemberId = clientId,
                            Topic = topic,
                            Timestamp = DateTime.UtcNow
                        };
                        
                        await BroadcastToTopic(topic, notification);
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
                logger.LogWarning($"No topic found: {topic}");
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
                logger.LogWarning($"No socket found for member: {memberId}");
                await RemoveFromTopic(topic, memberId);
                return;
            }

            if (!IsSocketAvailable(socket))
            {
                logger.LogWarning($"Socket not available for {memberId}");
                await RemoveFromTopic(topic, memberId);
                return;
            }

            try
            {
                SendToSocket(socket, message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error sending message to {memberId}");
                await RemoveFromTopic(topic, memberId);
            }
        }

        private string GetSocketId(IWebSocketConnection socket)
        {
            return socket.ConnectionInfo.Id.ToString();
        }

        private bool IsSocketAvailable(IWebSocketConnection socket)
        {
            return socket.IsAvailable;
        }

        private void SendToSocket<TMessage>(IWebSocketConnection socket, TMessage message) where TMessage : class
        {
            try
            {

                if (message is BaseDto baseDto)
                {
                    socket.SendDto(baseDto);
                }
                else
                {

                    var json = JsonSerializer.Serialize(message);
                    socket.Send(json);
                }
                
                logger.LogDebug($"Message sent to socket {GetSocketId(socket)}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error sending message to socket {GetSocketId(socket)}");
                throw;
            }
        }

        public async Task LogCurrentState()
        {
            try
            {
                logger.LogDebug(JsonSerializer.Serialize(new
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
                logger.LogError(ex, "Error logging current state");
            }
        }
    }

    public class MemberLeftNotification : BaseDto
    {
        public string MemberId { get; set; }
        public string Topic { get; set; }
        public DateTime Timestamp { get; set; }
    }
}