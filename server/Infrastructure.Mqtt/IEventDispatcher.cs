namespace Infrastructure.Mqtt;

public interface IEventDispatcher
{
    Task DispatchAsync(string topic, string payload);
}