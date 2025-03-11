namespace Application.Interfaces.Infrastructure.Mqtt;

/// <summary>
///     All handlers for publishing is triggered by direct invocation of the Publish method.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMqttPublisher<T>
{
    Task Publish(T dto);
}