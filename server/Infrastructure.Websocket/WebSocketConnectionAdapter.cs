using Fleck;
using service;

public class WebSocketConnection(IWebSocketConnection connection) : IConnection
{
    public void Send(string jsonSerializedMessage)
    {
        connection.Send(jsonSerializedMessage);
    }
    
}