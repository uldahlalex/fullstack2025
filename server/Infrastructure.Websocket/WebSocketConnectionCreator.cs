using Fleck;
using Infrastructure.Websocket;
using service;

public class WebSocketConnectionCreator : IConnectionCreator
{
    public IConnection Create(object nativeConnection)
    {
        if (nativeConnection is IWebSocketConnection wsConnection)
        {
            return new WebSocketConnectionAdapter(wsConnection);
        }
        
        throw new ArgumentException("Invalid connection type");
    }
}