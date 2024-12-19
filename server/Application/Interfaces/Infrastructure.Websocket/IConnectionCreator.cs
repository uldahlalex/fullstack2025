namespace Application.Interfaces.Infrastructure.Websocket;

public interface IConnectionCreator
{
    IConnection Create(object nativeConnection);
}