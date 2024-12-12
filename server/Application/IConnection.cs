namespace service;

public interface IConnection
{
    public void Send(string jsonSerialzedMessage);
    IConnection Create(object nativeConnection);

}