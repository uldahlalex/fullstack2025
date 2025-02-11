using Application.Interfaces;
using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers;

public class HumidityEventHandler : IMqttEventHandler<HumidityEventDto>
{
    private readonly ILogger<HumidityEventHandler> _logger;

    public HumidityEventHandler(
        IServiceLogic service,
        ILogger<HumidityEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(HumidityEventDto eventDtoData)
    {
        _logger.LogInformation("Humidity reading: {Humidity}% from sensor {SensorId}",
            eventDtoData.Humidity, eventDtoData.SensorId);

        //service.DoSomething()
    }
}