namespace Application.Interfaces.Infrastructure.Websocket;

public interface IConnection
{
    Guid Id { get; }
    void Send(string jsonSerializedMessage);
}