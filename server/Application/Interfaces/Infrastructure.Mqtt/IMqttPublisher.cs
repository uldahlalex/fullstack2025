namespace Application.Interfaces.Infrastructure.Mqtt;

public interface IMqttPublisher
{
    Task Publish(object dto, string topic);
}