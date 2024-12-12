using Fleck;
using service;

namespace Infrastructure.Websocket;

public class WebSocketConnectionCreator : IConnectionCreator
{
    public IConnection Create(object nativeConnection)
    {
        if (nativeConnection is IWebSocketConnection wsConnection) return new WebSocketConnectionAdapter(wsConnection);

        throw new ArgumentException("Invalid connection type");
    }
}