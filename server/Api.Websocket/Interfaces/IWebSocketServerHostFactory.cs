namespace Api.Websocket.Interfaces;

public interface IWebSocketServerHostFactory
{
    IWebSocketServerHost Create(WebApplication app);
}