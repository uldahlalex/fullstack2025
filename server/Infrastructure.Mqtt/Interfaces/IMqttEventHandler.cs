namespace Infrastructure.Mqtt.Interfaces;

/// <summary>
///     This is very much like the "BaseEventHandler" for websockets.
///     All traffic coming to the C# application from the broker should be guided to the correct event handler based on the
///     action matching the eventType in the DTO. So topic "device/A/DoThis" should to to the eventhandler taking in DTO called DoThis 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMqttEventHandler<in T> where T : IMqttEventDto
{
    Task HandleAsync(T eventData);
}