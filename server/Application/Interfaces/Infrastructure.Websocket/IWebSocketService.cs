namespace Application.Interfaces.Infrastructure.Websocket;

public interface IWebSocketService<T>
{
    public T RegisterConnection(T connection);
    T OnClose(T ws);
}