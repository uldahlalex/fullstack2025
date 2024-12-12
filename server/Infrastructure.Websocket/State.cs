using System.Collections.Concurrent;
using service;
using service.Interfaces;



public class State : IState
{
    private class ConnectionEntry
    {
        public Guid Id { get; }
        public IConnection Connection { get; }

        public ConnectionEntry(Guid id, IConnection connection)
        {
            Id = id;
            Connection = connection;
        }
    }

    private readonly ConcurrentDictionary<Guid, ConnectionEntry> _connections = new();
    
    public ConcurrentDictionary<Guid, IConnection> Connections => 
        new ConcurrentDictionary<Guid, IConnection>(_connections.ToDictionary(x => x.Key, x => x.Value.Connection));

    public bool TryAddConnection(IConnection connection)
    {
        var id = Guid.NewGuid();
        return _connections.TryAdd(id, new ConnectionEntry(id, connection));
    }

    public bool TryRemoveConnection(IConnection connection)
    {
        var entry = _connections.FirstOrDefault(x => x.Value.Connection.Equals(connection));
        return entry.Key != default && _connections.TryRemove(entry.Key, out _);
    }
}