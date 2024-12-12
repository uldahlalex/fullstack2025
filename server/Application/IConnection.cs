namespace service;

public interface IConnection
{
    void Send(string jsonSerializedMessage);
}
