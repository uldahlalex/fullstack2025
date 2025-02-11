namespace Api.Websocket.Interfaces;

public interface IWebSocketServerHost : IDisposable
{
    Task StartAsync(int port);
    Task StopAsync();
}