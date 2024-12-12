namespace service;

public interface IConnection
{
    Guid Id { get; }
    void Send(string jsonSerializedMessage);
}