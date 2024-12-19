using System.Collections.Concurrent;

namespace Application.Interfaces.Infrastructure.Websocket;

public interface IState
{
    ConcurrentDictionary<Guid, IConnection> Connections { get; }
    bool TryAddConnection(IConnection connection);
    bool TryRemoveConnection(IConnection connection);
}