using Application.Interfaces.Infrastructure.Broadcasting;
using Fleck;

namespace Infrastructure.Websocket;

public class WebSocketConnectionAdapter(IWebSocketConnection connection) : IConnection
{
    public Guid Id { get; } = connection.ConnectionInfo.Id;

    public void Send(string jsonSerializedMessage)
    {
        connection.Send(jsonSerializedMessage);
    }
}