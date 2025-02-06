public record TemperatureEvent : IMqttEvent
{
    public double Temperature { get; init; }
    public string SensorId { get; init; }
    public string Topic { get; init; }
    public DateTime Timestamp { get; init; }
}

public record HumidityEvent : IMqttEvent
{
    public double Humidity { get; init; }
    public string SensorId { get; init; }
    public string Topic { get; init; }
    public DateTime Timestamp { get; init; }
}