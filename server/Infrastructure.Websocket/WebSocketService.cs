using System.Collections.Concurrent;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;

namespace Infrastructure.Websocket;

public class WebSocketService : IWebSocketService<IWebSocketConnection>
{
    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _connections = new();

    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections =>
        new(_connections);

    public IWebSocketConnection RegisterConnection(IWebSocketConnection connection)
    {
        _connections.TryAdd(connection.ConnectionInfo.Id, connection);
        return connection;
    }

    public IWebSocketConnection OnClose(IWebSocketConnection ws)
    {
        throw new NotImplementedException();
    }

    public void Broadcast(string message)
    {
        foreach (var connection in _connections.Values) connection.Send(message);
    }

    public bool TryAddConnection(IWebSocketConnection connection)
    {
        return _connections.TryAdd(connection.ConnectionInfo.Id, connection);
    }

    public bool TryRemoveConnection(IWebSocketConnection connection)
    {
        return _connections.TryRemove(connection.ConnectionInfo.Id, out _);
    }
}