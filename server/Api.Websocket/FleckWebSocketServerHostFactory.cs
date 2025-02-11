using Api.Websocket;

public class FleckWebSocketServerHostFactory(ILogger<FleckWebSocketServerHost> logger) : IWebSocketServerHostFactory
{
    public IWebSocketServerHost Create(WebApplication app)
    {
        return new FleckWebSocketServerHost(app, logger);
    }
}