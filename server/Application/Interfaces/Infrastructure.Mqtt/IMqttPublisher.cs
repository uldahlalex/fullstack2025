namespace Application.Interfaces.Infrastructure.Mqtt;

public interface IMqttPublisher
{
    Task Publish(string topic, string payload);
}