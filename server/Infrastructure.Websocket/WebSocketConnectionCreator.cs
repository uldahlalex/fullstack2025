using Fleck;
using service;

public class WebSocketConnectionCreator : IConnectionCreator
{
    public IConnection Create(object nativeConnection)
    {
        if (nativeConnection is IWebSocketConnection wsConnection)
        {
            return new WebSocketConnection(wsConnection);
        }
        
        throw new ArgumentException("Invalid connection type");
    }
}