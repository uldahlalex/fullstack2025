namespace Infrastructure.Mqtt.Interfaces;

public interface IMqttEventHandler<in T> where T : IMqttEventDto
{
    Task HandleAsync(T eventData);
}