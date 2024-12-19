namespace Application.Interfaces.Infrastructure.Mqtt;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string topic, T payload);
    Task PublishAsync(string topic, string payload);
}