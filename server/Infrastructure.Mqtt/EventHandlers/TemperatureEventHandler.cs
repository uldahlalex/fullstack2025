using System.Text.Json;
using Application.Interfaces;
using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers;

public class TemperatureEventHandler(ILogger<TemperatureEventHandler> logger, IServiceLogic service)
    : IMqttEventHandler<TemperatureEventDto>
{
    public async Task HandleAsync(TemperatureEventDto eventDtoData)
    {
        logger.LogInformation("Temperature reading: {Temperature}°C from sensor {SensorId}",
            eventDtoData.Temperature, eventDtoData.SensorId);
        // service.Broadcast(JsonSerializer.Serialize(new { eventType = "temperature", eventDtoData.Temperature, eventDtoData.SensorId }),
        //     "temperature");
    }
}