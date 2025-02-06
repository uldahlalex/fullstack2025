public class TemperatureEventHandler(ILogger<TemperatureEventHandler> logger) : IMqttEventHandler<TemperatureEvent>
{
    public async Task HandleAsync(TemperatureEvent eventData)
    {
        logger.LogInformation("Temperature reading: {Temperature}°C from sensor {SensorId}",
            eventData.Temperature, eventData.SensorId);
    }
}

public class HumidityEventHandler : IMqttEventHandler<HumidityEvent>
{
    private readonly ILogger<HumidityEventHandler> _logger;

    public HumidityEventHandler(
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