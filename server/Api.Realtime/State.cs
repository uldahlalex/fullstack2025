using System.Collections.Concurrent;
using Fleck;

namespace realtimeapi;

public class State
{
    public ConcurrentDictionary<Guid, IWebSocketConnection> Connections { get; } = new();
}