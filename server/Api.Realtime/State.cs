using System.Collections.Concurrent;
using Fleck;

namespace Api.Realtime;

public class State
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
}