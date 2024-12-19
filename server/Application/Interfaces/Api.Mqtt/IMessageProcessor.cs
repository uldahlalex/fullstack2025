namespace Application.Interfaces.Api.Mqtt;

public interface IMessageProcessor
{
    Task ProcessAsync(Message message);
}

public interface IMessageDispatcher
{
    Task DispatchAsync(Message message, string topic);
}

public class Message
{
    public string MessageString { get; set; }
}