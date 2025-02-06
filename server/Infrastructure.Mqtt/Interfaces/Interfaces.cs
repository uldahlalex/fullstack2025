public interface IMqttEvent
{
    string Topic { get; }
    DateTime Timestamp { get; }
}

public interface IMqttEventHandler<in T> where T : IMqttEvent
{
    Task HandleAsync(T eventData);
}