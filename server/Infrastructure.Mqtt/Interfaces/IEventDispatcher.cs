namespace Infrastructure.Mqtt.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync(string topic, string payload);
}