using Api.Websocket;

public interface IWebSocketServerHostFactory
{
    IWebSocketServerHost Create(WebApplication app);
}