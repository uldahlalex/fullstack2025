namespace Infrastructure.Mqtt.Interfaces;

public interface IMqttEventHandler<in T> where T : IMqttEvent
{
    Task HandleAsync(T eventData);
}