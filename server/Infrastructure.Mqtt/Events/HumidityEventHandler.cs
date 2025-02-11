using Application.Interfaces;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.Events;

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