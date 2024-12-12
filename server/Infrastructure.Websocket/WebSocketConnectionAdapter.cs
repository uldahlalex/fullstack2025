using Fleck;
using service;

public class WebSocketConnectionAdapter(IWebSocketConnection fleckConnection) : IConnection
{

    public void Send(string jsonSerialzedMessage)
    {
        fleckConnection.Send(jsonSerialzedMessage);
    }
    public IConnection Create(object nativeConnection)
    {
        if (nativeConnection is IWebSocketConnection wsConnection)
        {
            return new IWebSocketConnection(wsConnection);
        }
        
        throw new ArgumentException("Invalid connection type");
    }
}