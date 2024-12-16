using System.Collections.Concurrent;
using Application;
using Application.Interfaces.Infrastructure.Broadcasting;

namespace Infrastructure.Websocket;

public class State : IState
{
    private readonly ConcurrentDictionary<Guid, IConnection> _connections = new();

    public ConcurrentDictionary<Guid, IConnection> Connections =>
        new(_connections);

    public bool TryAddConnection(IConnection connection)
    {
        return _connections.TryAdd(connection.Id, connection);
    }

    public bool TryRemoveConnection(IConnection connection)
    {
        return _connections.TryRemove(connection.Id, out _);
    }
}