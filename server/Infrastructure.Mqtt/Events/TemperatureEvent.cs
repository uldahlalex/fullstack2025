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

public class HumidityEventHandler : IMqttEventHandler<HumidityEvent>
{
    private readonly ILogger<HumidityEventHandler> _logger;

    public HumidityEventHandler(
        IServiceLogic service,
        ILogger<HumidityEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(HumidityEvent eventData)
    {
        _logger.LogInformation("Humidity reading: {Humidity}% from sensor {SensorId}",
            eventData.Humidity, eventData.SensorId);

        //service.DoSomething()
    }
}