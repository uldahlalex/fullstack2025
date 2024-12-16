namespace Application;

public interface IConnection
{
    Guid Id { get; }
    void Send(string jsonSerializedMessage);
}