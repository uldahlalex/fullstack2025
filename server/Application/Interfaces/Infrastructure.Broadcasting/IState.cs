using System.Collections.Concurrent;

namespace service.Interfaces;

public interface IState
{
    ConcurrentDictionary<Guid, IConnection> Connections { get; }
    bool TryAddConnection(IConnection connection);
    bool TryRemoveConnection(IConnection connection);
}