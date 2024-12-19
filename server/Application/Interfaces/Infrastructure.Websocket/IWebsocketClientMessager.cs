namespace Application.Interfaces.Infrastructure.Websocket;

public interface IWebsocketClientMessager
{
    void Broadcast(string jsonSerializedMessage, params Guid[] connectionIds);
}