namespace Api.Websocket;

public interface IWebSocketServerHost : IDisposable
{
    Task StartAsync(int port);
    Task StopAsync();
}