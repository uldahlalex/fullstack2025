using System.Collections.Concurrent;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;

namespace Infrastructure.Websocket;

public class WebSocketService : IWebSocketService<IWebSocketConnection> 
{
    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _connections = new();

    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections =>
        new(_connections);

    public bool TryAddConnection(IWebSocketConnection connection)
    {
        return _connections.TryAdd(connection.ConnectionInfo.Id, connection);
    }

    public bool TryRemoveConnection(IWebSocketConnection connection)
    {
        return _connections.TryRemove(connection.ConnectionInfo.Id, out _);
    }

    public IWebSocketConnection RegisterConnection(IWebSocketConnection connection)
    {
        throw new NotImplementedException();
    }
}