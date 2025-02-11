using Application.Interfaces;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.Events;

public class TemperatureEventHandler(ILogger<TemperatureEventHandler> logger, IServiceLogic service)
    : IMqttEventHandler<TemperatureEvent>
{
    public async Task HandleAsync(TemperatureEvent eventData)
    {
        logger.LogInformation("Temperature reading: {Temperature}°C from sensor {SensorId}",
            eventData.Temperature, eventData.SensorId);
        service.Broadcast(new { eventType = "temperature", eventData.Temperature, eventData.SensorId }, "temperature");
    }
}